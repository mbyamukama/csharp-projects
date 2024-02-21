using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace DiffieHellmanRaw
{
    class Program
    {
        public static void Main(string[] args)
        {

            DHKeyGroup alice = new DHKeyGroup(15);
            alice.DeriveKeyPair(); //sets private and public keys

            DHKeyGroup bob = new DHKeyGroup(alice.prime, alice.generator); //initialize using alice's same base values
            bob.DeriveKeyPair();

            BigInteger dhSecret1 = alice.ComputeSharedSecret(bob.PublicKey);
            BigInteger dhSecret2 = bob.ComputeSharedSecret(alice.PublicKey);

            Console.WriteLine("Alice Secret={0}\nBob Secret={1}", dhSecret1, dhSecret2);
            Console.WriteLine("is Equal={0}", dhSecret1 == dhSecret2)
;

        }
    }
}