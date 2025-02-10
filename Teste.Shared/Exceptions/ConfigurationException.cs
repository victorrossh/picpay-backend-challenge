namespace Teste.Shared.Exceptions;

public class ConfigurationException(string[]? messages = null!) : Exception(string.Join(", ", messages ?? []));