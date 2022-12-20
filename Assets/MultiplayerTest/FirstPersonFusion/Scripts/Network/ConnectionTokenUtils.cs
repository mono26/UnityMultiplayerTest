using System;


/// <summary>
/// Fusion Connection Token Utility methods
/// </summary>
public static class ConnectionTokenUtils
{
    /// <summary>
    /// Create new random token
    /// </summary>
    public static byte[] NewToken() => Guid.NewGuid().ToByteArray();

    /// <summary>
    /// Converts a token into a Hash format
    /// </summary>
    /// <param name="token">Token to be hashed</param>
    /// <returns>Token hash</returns>
    public static int HashToToken(byte[] token) => new Guid(token).GetHashCode();

    /// <summary>
    /// Converts a Token into a string
    /// </summary>
    /// <param name="token">Token to be parsed</param>
    /// <returns>Token as a string</returns>
    public static string TokenToString(byte[] token) => new Guid(token).ToString();
}
