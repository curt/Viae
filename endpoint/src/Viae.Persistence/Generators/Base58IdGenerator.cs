// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Persistence.Generators;

/// <summary>
/// Generates Base58-encoded flake-like identifiers.
/// </summary>
public class Base58IdGenerator
{
    private const string Base58Alphabet =
        "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    /// <summary>
    /// Generates a new Base58 identifier.
    /// </summary>
    /// <returns>A Base58-encoded identifier string.</returns>
    public static string Generate()
    {
        // TODO: Implement flake-like ID generation with timestamp and random components
        // For now, using a simple placeholder
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var random = Random.Shared.Next();
        var combined = (timestamp << 32) | (uint)random;

        return EncodeBase58(BitConverter.GetBytes(combined));
    }

    private static string EncodeBase58(byte[] data)
    {
        // Simple Base58 encoding implementation
        // TODO: Implement proper Base58 encoding
        // BigInteger constructor interprets bytes as little-endian with sign bit
        // Add 0x00 byte to ensure positive interpretation
        var dataWithSign = new byte[data.Length + 1];
        Array.Copy(data, dataWithSign, data.Length);
        dataWithSign[data.Length] = 0x00; // Ensure positive

        var value = new System.Numerics.BigInteger(dataWithSign);
        var result = string.Empty;

        // Handle zero case
        if (value == 0)
        {
            return Base58Alphabet[0].ToString();
        }

        while (value > 0)
        {
            var remainder = (int)(value % 58);
            value /= 58;
            result = Base58Alphabet[remainder] + result;
        }

        return result;
    }
}
