using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CryptographicAlgorithms.Cyphers
{
    class RSA
    {
        public int KeySize { get; private set; }
        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }
        public BigInteger N { get; private set; }
        public BigInteger Phi { get; private set; }
        public BigInteger E { get; private set; }
        public BigInteger D { get; private set; }

        /// <summary>
        /// Creates instance of RSA cryptography algorithm class
        /// </summary>
        /// <param name="keySize">Key size in bits</param>
        public RSA(int keySize)
        {
            if (keySize < 16)
            {
                throw new Exception("Bad key size");
            }
            KeySize = keySize;
        }

        /// <summary>
        /// Creates instance of RSA cryptography algorithm class
        /// </summary>
        /// <param name="p">Prime number P</param>
        /// <param name="q">Prime number Q</param>
        public RSA(BigInteger p, BigInteger q)
        {
            P = p;
            Q = q;

            KeySize = (p > q) ? (p.ToByteArray().Length * 8) : (q.ToByteArray().Length * 8);
        }

        /// <summary>
        /// Generates random prime values of p and q
        /// </summary>
        public void GeneratePQ()
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
            BigInteger q = new BigInteger(bytes);
            while (!q.IsProbablePrime(KeySize) || q == p)
            {
                rng.GetBytes(bytes);

                byte[] temp = new byte[bytes.Length + 1];
                Array.Copy(bytes, temp, bytes.Length);

                q = new BigInteger(temp);
            }

            P = p;
            Q = q;
        }

        /// <summary>
        /// Calculates N and φ(n)
        /// </summary>
        public void CalculateNPhi()
        {
            if (!P.IsZero && !Q.IsZero)
            {
                N = P * Q;
                Phi = (P - 1) * (Q - 1);
            }
        }

        /// <summary>
        /// Calculates private key and public key
        /// </summary>
        public void CalculateDE()
        {
            byte[] bytes = new byte[KeySize / 8];
            BigInteger e = new BigInteger(bytes);

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            while (e >= Phi || EuclideanGCD(Phi, e) != 1)
            {
                rng.GetBytes(bytes);

                byte[] temp = new byte[bytes.Length + 1];
                Array.Copy(bytes, temp, bytes.Length);

                e = new BigInteger(temp);
            }
            E = e;

            BigInteger d = ExtendedEuclidean(e, Phi);
            D = d;
        }

        /// <summary>
        /// Sets values of N, public key, secret key
        /// </summary>
        /// <param name="n">N</param>
        /// <param name="e">Public key</param>
        /// <param name="d">Secret key</param>
        public void SetNED(BigInteger n, BigInteger e, BigInteger d)
        {
            N = n;
            E = e;
            D = d;
        }

        /// <summary>
        /// Enrypts message using public key
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Encrypted message</returns>
        public byte[] EncryptMessage(byte[] bytes)
        {
            List<byte> res = new List<byte>();
            int blockSize = N.ToByteArray().Length;

            for (int i = 0; i < bytes.Length; i++)
            {
                byte[] block = new byte[blockSize];

                BigInteger numMessage = new BigInteger(bytes[i]);
                //BigInteger numEncryptedMessage = BigInteger.ModPow(numMessage, E, N);
                BigInteger numEncryptedMessage = modulo(numMessage, E, N);
                Array.Copy(numEncryptedMessage.ToByteArray(), block, numEncryptedMessage.ToByteArray().Length);

                res.AddRange(block);
            }
            return res.ToArray();
        }

        /// <summary>
        /// Decrypts message using secret key
        /// </summary>
        /// <param name="encMessage">Encrypted message</param>
        /// <returns>Decrypted message</returns>
        public byte[] DecryptMessage(byte[] bytes)
        {
            int blockSize = N.ToByteArray().Length;
            List<byte> res = new List<byte>();

            for (int i = 0; i < bytes.Length; i += blockSize)
            {
                byte[] block = new byte[blockSize];
                Array.Copy(bytes, i, block, 0, i + blockSize > bytes.Length ? bytes.Length - i : blockSize);

                BigInteger numMessage = new BigInteger(block);
                //BigInteger numEncryptedMessage = BigInteger.ModPow(numMessage, E, N);
                BigInteger numDecryptedMessage = modulo(numMessage, D, N);


                res.AddRange(numDecryptedMessage.ToByteArray());
            }

            return res.ToArray();
        }

        private BigInteger EuclideanGCD(BigInteger a, BigInteger b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            if (a == 0)
                return b;
            else
                return a;
        }

        private BigInteger ExtendedEuclidean(BigInteger a, BigInteger b)
        {
            BigInteger dividend = a % b;
            BigInteger divisor = b;

            BigInteger last_x = BigInteger.One;
            BigInteger curr_x = BigInteger.Zero;

            while (divisor.Sign > 0)
            {
                BigInteger quotient = dividend / divisor;
                BigInteger remainder = dividend % divisor;
                if (remainder.Sign <= 0)
                {
                    break;
                }

                BigInteger next_x = last_x - quotient * curr_x;
                last_x = curr_x;
                curr_x = next_x;

                dividend = divisor;
                divisor = remainder;
            }

            if (divisor != BigInteger.One)
            {
                throw new Exception("Given numbers are not coprimes");
            }
            return (curr_x.Sign < 0 ? curr_x + b : curr_x);
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
