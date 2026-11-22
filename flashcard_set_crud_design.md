# Flashcard Set CRUD: Design Notes (Quizlet-style)

Scope: create, read, update, and delete for flashcard sets, plus study sessions, hard-delete privacy rules, document indexing, and backups. Business analysis of Quizlet is out of scope.

> **Status of this document.** The domain model and schema below are a *design proposal*, not Quizlet's actual (non-public) schema. The Quizlet behavior described comes from help-center pages and tutorials and may have changed. The privacy section is engineering guidance, not legal advice.

---

## 1. How Quizlet handles set CRUD (observed behavior)

| Operation | Behavior |
|---|---|
| **Create** | From the Create button: title, description, term/definition pairs. Also spreadsheet import, AI generation from uploaded notes, and "Save and Edit" (copy of someone else's set). Free users can create sets. Diagram sets are premium. |
| **Read** | Set page and five study modes (Flashcards, Learn, Test, Match, Q-Chat). Search with filters. Default visibility lets anyone view; only the owner edits. Folders and tags organize sets; only teachers create classes. |
| **Update** | Edit title, description, terms, and order. Owner can allow others to edit (password-protected editing, or "Allow class to edit"). Others edit via a copy. |
| **Delete** | Owner deletes from the set menu and confirms. One low-quality source says deletion is not recoverable; confirm in-app. |

---

## 2. Domain model

### Relationships

```
User 1 ──── N FlashcardSet          (owner)
FlashcardSet 1 ──── N Flashcard

User 1 ──── N StudySession
FlashcardSet 1 ──── N StudySession  (which set was studied)

StudySession 1 ──── N CardReview ──── N:1 Flashcard
```

- A set has one or many cards (zero is possible while editing).
- Starting to study creates a new `StudySession` (one run through a set in one mode).
- A card does **not** belong to a session directly. `StudySession <-> Flashcard` is many-to-many through `CardReview`, which records the result of each card attempt. A card can be reviewed several times in one session (e.g. Learn mode).
- Sessions belong to a **user**; progress is meaningless without one.

### Entities and key fields

| Entity | Key fields |
|---|---|
| `app_user` | id, username, email |
| `flashcard_set` | id, owner_id, title, description, visibility, source_set_id (copy lineage), timestamps |
| `flashcard` | id, set_id, term, definition, position, timestamps |
| `study_session` | id, user_id, set_id, mode, started_at, ended_at |
| `card_review` | id, session_id, card_id, result, response_time_ms, reviewed_at |
| `user_card_progress` (optional) | user_id, card_id, mastery_level, correct_streak, next_review_at. Current state per user and card; `card_review` is the history |

---

## 3. Delete policy: decision history

1. **First proposal (soft delete):** `deleted_at` on sets and cards. Cards soft-deleted so `CardReview` history stays valid; sets soft-deleted so data could feed document indexing.
2. **Privacy review:** soft delete is not erasure. Users who delete a set generally expect it gone, so the design moved to **hard delete**.

### Final hard-delete rules

| You delete | What happens |
|---|---|
| **A user** | Their sets, cards, sessions, reviews, and progress are removed |
| **A set** | Its cards, **all** sessions on it (including other users'), and those sessions' reviews are removed |
| **A card** | Its `user_card_progress` rows are removed. Its `card_review` rows stay with `card_id = NULL` (result and timing only, no content) |

Notes:
- No table other than `flashcard` stores card text, so nothing readable survives a delete.
- To drop review history entirely when a card is deleted, change `ON DELETE SET NULL` to `ON DELETE CASCADE` on `card_review.card_id`.
- Trade-off: deleting a set removes other users' study history on it. This is the cleanest privacy outcome; the alternative is `ON DELETE SET NULL` on `study_session.set_id`.
- Copies made with "Save and Edit" are independent sets; deleting the original leaves them intact (`source_set_id` is set to NULL).

---

## 4. Full PostgreSQL schema

Tested on PostgreSQL 16. Verified behaviors: schema loads without errors; a review referencing a card from another set is rejected; card positions can be swapped inside one transaction (deferred unique constraint); deleting a card nulls its reviews' `card_id` and removes its progress; deleting a set leaves zero cards, sessions, and reviews; the outbox records delete events for cards and the set.

```sql
-- =============================================================================
-- Flashcard sets + study sessions  |  PostgreSQL 14+  |  HARD DELETE (privacy)
--
-- Delete behavior
--   Delete a user       -> their sets, cards, sessions, reviews, progress are removed
--   Delete a set        -> its cards, ALL sessions on it, and their reviews are removed
--   Delete a card       -> its progress rows are removed; its review rows are kept
--                          with card_id = NULL (result/time stats only, no content)
-- Nothing in the review/session tables stores card text, so no content survives.
-- =============================================================================

BEGIN;

-- ---------- Types ------------------------------------------------------------
CREATE TYPE set_visibility AS ENUM ('private', 'link', 'public');
CREATE TYPE study_mode     AS ENUM ('flashcards', 'learn', 'test', 'match');
CREATE TYPE review_result  AS ENUM ('correct', 'incorrect', 'skipped');

-- ---------- Shared trigger: keep updated_at fresh ----------------------------
CREATE FUNCTION set_updated_at() RETURNS trigger AS $$
BEGIN
  NEW.updated_at := now();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ---------- app_user ---------------------------------------------------------
CREATE TABLE app_user (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  username   TEXT        NOT NULL CHECK (length(btrim(username)) > 0),
  email      TEXT        NOT NULL CHECK (position('@' IN email) > 1),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
  updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
CREATE UNIQUE INDEX uq_app_user_username ON app_user (lower(username));
CREATE UNIQUE INDEX uq_app_user_email    ON app_user (lower(email));

-- ---------- flashcard_set ----------------------------------------------------
CREATE TABLE flashcard_set (
  id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  owner_id      BIGINT         NOT NULL REFERENCES app_user (id) ON DELETE CASCADE,
  title         TEXT           NOT NULL CHECK (length(btrim(title)) > 0),
  description   TEXT,
  visibility    set_visibility NOT NULL DEFAULT 'private',
  -- "Save and Edit" copies: keep lineage, but deleting the original must not
  -- delete the copy.
  source_set_id BIGINT REFERENCES flashcard_set (id) ON DELETE SET NULL,
  created_at    TIMESTAMPTZ    NOT NULL DEFAULT now(),
  updated_at    TIMESTAMPTZ    NOT NULL DEFAULT now()
);
CREATE INDEX ix_set_owner      ON flashcard_set (owner_id);
CREATE INDEX ix_set_source     ON flashcard_set (source_set_id) WHERE source_set_id IS NOT NULL;
CREATE INDEX ix_set_public     ON flashcard_set (created_at DESC) WHERE visibility = 'public';

CREATE TRIGGER trg_set_updated_at
  BEFORE UPDATE ON flashcard_set
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ---------- flashcard --------------------------------------------------------
CREATE TABLE flashcard (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  set_id     BIGINT      NOT NULL REFERENCES flashcard_set (id) ON DELETE CASCADE,
  term       TEXT        NOT NULL CHECK (length(btrim(term)) > 0),
  definition TEXT        NOT NULL CHECK (length(btrim(definition)) > 0),
  position   INT         NOT NULL CHECK (position >= 0),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  -- Deferred so cards can be reordered/swapped inside one transaction.
  CONSTRAINT uq_card_position UNIQUE (set_id, position) DEFERRABLE INITIALLY DEFERRED
);
-- (set_id, position) unique constraint already indexes set_id lookups.

CREATE TRIGGER trg_card_updated_at
  BEFORE UPDATE ON flashcard
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ---------- study_session ----------------------------------------------------
-- One row each time a user starts studying a set in a given mode.
CREATE TABLE study_session (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  user_id    BIGINT      NOT NULL REFERENCES app_user (id)      ON DELETE CASCADE,
  set_id     BIGINT      NOT NULL REFERENCES flashcard_set (id) ON DELETE CASCADE,
  mode       study_mode  NOT NULL,
  started_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  ended_at   TIMESTAMPTZ,
  CHECK (ended_at IS NULL OR ended_at >= started_at)
);
CREATE INDEX ix_session_user_started ON study_session (user_id, started_at DESC);
CREATE INDEX ix_session_set          ON study_session (set_id);

-- ---------- card_review ------------------------------------------------------
-- Join entity: Flashcard <-> StudySession is many-to-many through this table.
-- A card may be reviewed several times in one session (e.g. Learn mode).
CREATE TABLE card_review (
  id               BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  session_id       BIGINT        NOT NULL REFERENCES study_session (id) ON DELETE CASCADE,
  card_id          BIGINT        REFERENCES flashcard (id) ON DELETE SET NULL,
  result           review_result NOT NULL,
  response_time_ms INT           CHECK (response_time_ms >= 0),
  reviewed_at      TIMESTAMPTZ   NOT NULL DEFAULT now()
);
CREATE INDEX ix_review_session ON card_review (session_id);
CREATE INDEX ix_review_card    ON card_review (card_id) WHERE card_id IS NOT NULL;

-- A reviewed card must belong to the set the session is studying.
CREATE FUNCTION check_review_card_in_session_set() RETURNS trigger AS $$
BEGIN
  IF NEW.card_id IS NOT NULL AND NOT EXISTS (
       SELECT 1
       FROM study_session s
       JOIN flashcard f ON f.set_id = s.set_id
       WHERE s.id = NEW.session_id AND f.id = NEW.card_id
     ) THEN
    RAISE EXCEPTION 'card % does not belong to the set of session %',
      NEW.card_id, NEW.session_id USING ERRCODE = 'foreign_key_violation';
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_review_card_in_set
  BEFORE INSERT OR UPDATE OF card_id, session_id ON card_review
  FOR EACH ROW EXECUTE FUNCTION check_review_card_in_session_set();

-- ---------- user_card_progress (optional: mastery / spaced repetition) -------
-- Current state per user+card. card_review is the history; this is the summary.
-- Deleted with the card, since progress on a nonexistent card is meaningless.
CREATE TABLE user_card_progress (
  user_id          BIGINT   NOT NULL REFERENCES app_user (id)  ON DELETE CASCADE,
  card_id          BIGINT   NOT NULL REFERENCES flashcard (id) ON DELETE CASCADE,
  mastery_level    SMALLINT NOT NULL DEFAULT 0 CHECK (mastery_level BETWEEN 0 AND 5),
  correct_streak   INT      NOT NULL DEFAULT 0 CHECK (correct_streak >= 0),
  last_reviewed_at TIMESTAMPTZ,
  next_review_at   TIMESTAMPTZ,
  PRIMARY KEY (user_id, card_id)
);
CREATE INDEX ix_progress_due ON user_card_progress (user_id, next_review_at);

-- =============================================================================
-- Operational notes
--   * Purge the outbox on a schedule, e.g.:
--       DELETE FROM index_outbox WHERE processed_at < now() - interval '7 days';
--   * Hard delete also has to reach: the search index, caches, replicas, logs
--     and backups (let backups expire on a fixed retention window).
--   * Reorder cards inside one transaction (the position constraint is deferred):
--       BEGIN;
--       UPDATE flashcard SET position = 1 WHERE id = :a;
--       UPDATE flashcard SET position = 0 WHERE id = :b;
--       COMMIT;
-- =============================================================================
```

---

## 6. Backups and privacy

Deleting rows only fixes the primary database. The same text may remain in the search index, caches, replicas, logs, and backups.

- **Search index:** handled by the outbox (section 4), provided the indexer keeps running.
- **Caches and replicas:** set sensible expiry; check lag.
- **Logs:** never log card content.
- **Backups:** you can't edit a snapshot record by record, so the accepted approach is: delete from live systems immediately, put backup copies *beyond use*, and let a documented retention schedule overwrite them.

### Three commitments

1. **Fixed retention window** (e.g. 30 days) covering every backup type, including WAL archives. Shorter means data disappears sooner but leaves less recovery time.
2. **Beyond use:** nobody queries or browses backups; they are encrypted, access-restricted and logged, and used only for disaster recovery.
3. **Post-restore re-delete.** Restoring an old backup resurrects data deleted since. After any restore, re-run all deletions recorded since the backup was taken.

### Deletion log (add to the schema)

The outbox is purged after days, which is shorter than the backup window, so keep a separate long-lived log of **ids only**:

```sql
CREATE TABLE deletion_log (
  entity_type TEXT        NOT NULL CHECK (entity_type IN ('user','flashcard_set','flashcard')),
  entity_id   BIGINT      NOT NULL,
  deleted_at  TIMESTAMPTZ NOT NULL DEFAULT now(),
  PRIMARY KEY (entity_type, entity_id)
);
-- Keep rows at least as long as the longest backup, then purge:
-- DELETE FROM deletion_log WHERE deleted_at < now() - interval '35 days';
```

- Identity ids are never reused, so replaying deletes cannot remove the wrong rows.
- The log must live outside the restored database (or come from a source newer than the backup), or the restore erases the list of things to delete.
- Cascades handle children: re-deleting a set removes its cards and sessions.
- Populate it with delete triggers on `app_user`, `flashcard_set`, and `flashcard` (not yet in the schema file).

### Sample privacy-policy wording (adapt with advice)

> When you delete a study set or your account, we remove it from our live systems immediately. Encrypted backup copies may remain for up to 30 days before being automatically overwritten. During that time we don't use backup data for any purpose other than disaster recovery, and if we restore from a backup we delete your data again.

The same facts belong in any reply to a user who asks for confirmation of deletion: live data deleted, copies remain in backups, the date they'll be overwritten, and no other use in the meantime.

### Regulatory note

The UK ICO accepts "beyond use" until backups are overwritten on a documented schedule. EU authorities have not all taken the same line; French guidance reportedly allows temporary retention when clearly disclosed with the retention period. GDPR Article 5(2) makes you responsible for demonstrating compliance, so keep written backup policies and restore-test records. CCPA and other regimes differ. Confirm which apply with legal counsel.

---

## 7. Checklist

- [ ] Load the schema and add `deletion_log` plus its triggers
- [ ] Run an indexer worker that consumes `index_outbox`
- [ ] Schedule outbox and deletion-log purges
- [ ] Configure backup retention (including WAL archives) and encryption
- [ ] Restrict and log backup access
- [ ] Write and test the post-restore re-delete step in the runbook
- [ ] Never log card content
- [ ] Add the backup-retention statement to the privacy policy
- [ ] Keep records of backup policy and restore tests
- [ ] Confirm applicable regulations with counsel

## 8. Open decisions

- Backup retention length (30 days is typical; weigh against recovery needs)
- Whether deleting a set should delete other users' sessions or keep them with `set_id = NULL`
- Whether to keep anonymized review stats after a card is deleted
- Whether private sets should ever reach the indexing pipeline
- Technology stack for the CRUD API (not yet chosen)
