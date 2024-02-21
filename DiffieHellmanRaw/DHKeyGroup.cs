using System.Globalization;
using System.Numerics;

namespace DiffieHellmanRaw
{
    /// <summary>
    /// @Author: Maximus Byamukama, PhD
    /// Refer to https://www.ietf.org/rfc/rfc2409.txt and https://www.ietf.org/rfc/rfc3526.txt for standard DH prime groups and their generators
    /// </summary>
    public class DHKeyGroup
    {
        public BigInteger prime;//modulus
        private BigInteger PrivateKey = BigInteger.Zero;
        public  BigInteger PublicKey { get; private set; }
        public int generator; //base (generator)
        private int[] RfcAllowedGroupIds = { 1, 2, 5, 14, 15, 16, 17, 18 };

        /// <summary>
        /// Creates a DHKeyHelper object with a known prime (modulus) and known generator (base). 
        /// </summary>
        /// <param name="prime"></param>
        /// <param name="generator"></param>
        public DHKeyGroup(BigInteger prime, int generator)
        {
            this.prime = prime;
            this.generator = generator;
        }
        public DHKeyGroup(int dhGroupId) //see RFC3526. NB: Positive BigInt hex values in C# must have a leading 0
        {
            if (!RfcAllowedGroupIds.Contains<int>(dhGroupId)) throw new ArgumentException("The provided DH group ID is invalid.");
            if (dhGroupId == 16 || dhGroupId == 17 || dhGroupId == 18) throw new NotSupportedException("4096-bit and higher MODP Groups are currently not supported.");
            switch (dhGroupId)
            {
                case 1: //768-bit MODP Group 1. (1st Oakley group) The prime is: 2^768 - 2 ^704 - 1 + 2^64 * { [2^638 pi] + 149686 }
                    prime = BigInteger.Parse("0FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74"+
                                         "020BBEA63B139B22514A08798E3404DDEF9519B3CD3A431B302B0A6DF25F1437"+
                                         "4FE1356D6D51C245E485B576625E7EC6F44C42E9A63A3620FFFFFFFFFFFFFFFF",NumberStyles.AllowHexSpecifier);
                    generator = 2;
                    break;
                case 2: //1024-bit MODP Group id 2. (2nd Oakley group) The prime is: 2^1024 - 2^960 - 1 + 2^64 * { [2^894 pi] + 129093 }
                    prime = BigInteger.Parse("0FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74"+
                                         "020BBEA63B139B22514A08798E3404DDEF9519B3CD3A431B302B0A6DF25F1437"+
                                         "4FE1356D6D51C245E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7ED"+
                                         "EE386BFB5A899FA5AE9F24117C4B1FE649286651ECE65381FFFFFFFFFFFFFFFF", NumberStyles.AllowHexSpecifier);
                    generator = 2;
                    break;
                case 5: //1536-bit MODP Group id 5. The prime is: 2^1536 - 2^1472 - 1 + 2^64 * { [2^1406 pi] + 741804 }
                    prime = BigInteger.Parse("0FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74020BBEA63B139B22514A08798E3404DD"+
                                              "EF9519B3CD3A431B302B0A6DF25F14374FE1356D6D51C245E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7ED"+
                                              "EE386BFB5A899FA5AE9F24117C4B1FE649286651ECE45B3DC2007CB8A163BF0598DA48361C55D39A69163FA8FD24CF5F"+
                                              "83655D23DCA3AD961C62F356208552BB9ED529077096966D670C354E4ABC9804F1746C08CA237327FFFFFFFFFFFFFFFF", NumberStyles.AllowHexSpecifier);
                    generator = 2;
                    break;
                case 14: //2048-bit MODP Group id 14   This prime is: 2 ^ 2048 - 2 ^ 1984 - 1 + 2 ^ 64 * { [2 ^ 1918 pi] +124476 }
                    prime = BigInteger.Parse("0FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74020BBEA63B139B22514A08798E3404DDEF9519B3CD3A431B302B0A6DF25F1437"+
                                             "4FE1356D6D51C245E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7EDEE386BFB5A899FA5AE9F24117C4B1FE649286651ECE45B3DC2007CB8A163BF05"+
                                             "98DA48361C55D39A69163FA8FD24CF5F83655D23DCA3AD961C62F356208552BB9ED529077096966D670C354E4ABC9804F1746C08CA18217C32905E462E36CE3B"+
                                             "E39E772C180E86039B2783A2EC07A28FB5C55DF06F4C52C9DE2BCBF6955817183995497CEA956AE515D2261898FA051015728E5A8AACAA68FFFFFFFFFFFFFFFF", NumberStyles.AllowHexSpecifier);
                    generator = 2;
                    break;
                case 15: //3072-bit MODP Group id 15. This prime is: 2 ^ 3072 - 2 ^ 3008 - 1 + 2 ^ 64 * { [2 ^ 2942 pi] +1690314 }

                    prime = BigInteger.Parse("0FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74020BBEA63B139B22514A08798E3404DDEF9519B3CD3A431B302B0A6DF25F1437"+
                                             "4FE1356D6D51C245E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7EDEE386BFB5A899FA5AE9F24117C4B1FE649286651ECE45B3DC2007CB8A163BF05"+
                                             "98DA48361C55D39A69163FA8FD24CF5F83655D23DCA3AD961C62F356208552BB9ED529077096966D670C354E4ABC9804F1746C08CA18217C32905E462E36CE3B"+
                                             "E39E772C180E86039B2783A2EC07A28FB5C55DF06F4C52C9DE2BCBF6955817183995497CEA956AE515D2261898FA051015728E5A8AAAC42DAD33170D04507A33"+
                                             "A85521ABDF1CBA64ECFB850458DBEF0A8AEA71575D060C7DB3970F85A6E1E4C7ABF5AE8CDB0933D71E8C94E04A25619DCEE3D2261AD2EE6BF12FFA06D98A0864"+
                                             "D87602733EC86A64521F2B18177B200CBBE117577A615D6C770988C0BAD946E208E24FA074E5AB3143DB5BFCE0FD108E4B82D120A93AD2CAFFFFFFFFFFFFFFFF", NumberStyles.AllowHexSpecifier);
                    generator = 2;
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// Derives the public-private key pair
        /// </summary>
        /// <returns>the public key</returns>
        public BigInteger DeriveKeyPair()
        {
            PrivateKey = RandomBigInteger.Next(prime);
            PublicKey = BigInteger.ModPow(generator, PrivateKey, prime); //g^pk % p
            return PublicKey;
        }
        public BigInteger ComputeSharedSecret(BigInteger remotePublicKey)
        {
            if (remotePublicKey < 0 || remotePublicKey == BigInteger.Zero) throw new ArgumentException("The provided remote public key is invalid");
            if (PrivateKey == BigInteger.Zero) throw new InvalidOperationException("The private key must be defined before this method call");
            return BigInteger.ModPow(remotePublicKey, PrivateKey, prime);
        }

    }
}