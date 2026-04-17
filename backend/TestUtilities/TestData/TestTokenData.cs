using Logic.User;

namespace TestUtilities.TestData;

public static class TestTokenData
{
	private const string TestValidTokenSecretKey = "test-valid-secret-key-123451234567876543456787654";

	public static readonly TokenGeneratorConfig TestValidTokenConfig = new()
	{
		ExpiryMinutes = 60,
		SecretKey = TestValidTokenSecretKey
	};
}