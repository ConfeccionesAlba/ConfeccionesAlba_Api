namespace ConfeccionesAlba_Api.Configurations;

public record JwtSettings(string SecretKey, string Issuer, string Audience, int ExpirationInMinutes);