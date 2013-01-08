using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CryptographicAlgorithms.Cyphers
{
    class Diffi_Hellman
    {
        public int KeySize { get; private set; }
        public BigInteger P { get; private set; }
        public BigInteger G { get; private set; }
        public BigInteger a { get; private set; }
        public BigInteger b { get; private set; }
        public BigInteger A { get; private set; }
        public BigInteger B { get; private set; }
        public BigInteger Key1 { get; private set; }
        public BigInteger Key2 { get; private set; }

        public Diffi_Hellman(int keySize)
        {
            if (keySize < 16)
            {
                throw new Exception("Bad key size");
            }
            KeySize = keySize;
        }

        public Diffi_Hellman(BigInteger p, BigInteger g)
        {
            P = p;
            G = g;
            KeySize = p.ToByteArray().Length * 8;
        }

        public void GeneratePG()
        {
            byte[] bytes = new byte[KeySize / 8];
            BigInteger p = new BigInteger(bytes);

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            while (!p.IsProbablePrime(KeySize))
            {
                rng.GetBytes(bytes);

                byte[] temp = new byte[bytes.Length + 1];
                Array.Copy(bytes, temp, bytes.Length);

                p = new BigInteger(temp);
            }

            bytes = new byte[KeySize / 8];
            BigInteger g = new BigInteger(bytes);
            while (!g.IsProbablePrime(KeySize) || g >= p)
            {
                rng.GetBytes(bytes);

                byte[] temp = new byte[bytes.Length + 1];
                Array.Copy(bytes, temp, bytes.Length);

                g = new BigInteger(temp);
            }

            P = p;
            G = g;
        }

        public void Generate_ab()
        {
            byte[] bytes = new byte[KeySize / 8];
            BigInteger _a = new BigInteger(bytes);

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            while (!_a.IsProbablePrime(KeySize) || _a >= P)
            {
                rng.GetBytes(bytes);

                byte[] temp = new byte[bytes.Length + 1];
                Array.Copy(bytes, temp, bytes.Length);

                _a = new BigInteger(temp);
            }

            bytes = new byte[KeySize / 8];
            BigInteger _b = new BigInteger(bytes);
            while (!_b.IsProbablePrime(KeySize) || _b >= P)
            {
                rng.GetBytes(bytes);

                byte[] temp = new byte[bytes.Length + 1];
                Array.Copy(bytes, temp, bytes.Length);

                _b = new BigInteger(temp);
            }

            a = _a;
            b = _b;
        }

        public void Set_b(BigInteger _b)
        {
            b = _b;
        }

        public void Set_a(BigInteger _a)
        {
            a = _a;
        }

        public void SetB(BigInteger b)
        {
            B = b;
        }

        public void SetA(BigInteger a)
        {
            A = a;
        }

        public void CalculateA()
        {
            A = modulo(G, a, P);
        }

        public void CalculateB()
        {
            B = modulo(G, b, P);
        }

        public void CalculateKey1()
        {
            BigInteger key = modulo(B, a, P);
            Key1 = key;
        }

        public void CalculateKey2()
        {
            BigInteger key = modulo(A, b, P);
            Key2 = key;
        }

        BigInteger modulo(BigInteger a, BigInteger b, BigInteger c)
        {
            BigInteger x = 1, y = a;
            while (b > 0)
            {
                if (b % 2 == 1)
                {
                    x = (x * y) % c;
                }
                y = (y * y) % c;
                b /= 2;
            }
            return x % c;
        }
    }
}
