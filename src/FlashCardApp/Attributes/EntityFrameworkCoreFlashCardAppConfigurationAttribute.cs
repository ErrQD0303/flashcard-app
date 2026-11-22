namespace FlashCardApp.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EntityFrameworkCoreFlashCardAppConfigurationAttribute(string domain) : Attribute
{
    public string Domain { get; } = domain;
}