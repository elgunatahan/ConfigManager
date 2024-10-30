namespace ConfigurationLibrary.Interfaces
{
    public interface ITypeParser
    {
        T Parse<T>(string value);
    }
}
