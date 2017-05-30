using System;
using System.Text;
using UnityEngine;
#if NETFX_CORE
    using Windows.Security.Cryptography.Core;
    using Windows.Security.Cryptography;
    using Windows.Storage.Streams;
#endif

public class PairingCryptoManager : MonoBehaviour
{

#if NETFX_CORE
    // Open the algorithm provider for the specified asymmetric algorithm.
    private AsymmetricKeyAlgorithmProvider objAlgProv;
//    private KeyDerivationAlgorithmProvider objKdfProv; // uncomment if we start using symmetric key derivation from a shared secret
    private CryptographicKey myKeyPair;
#endif
    private string myPublicKeyString = "";

    // --------- HASHING -------------------

    public string generateRandomNonce()
    {
#if NETFX_CORE
        IBuffer buffNonce = CryptographicBuffer.GenerateRandom(32);
        Debug.Log("NONCE: " + cryptoBufferToString(buffNonce));
        return cryptoBufferToString(buffNonce);
#else
        System.Random rand = new System.Random();
        string newNonce = "nonce_" + rand.Next().ToString();
        return newNonce;
#endif
    }

    public string generateHashFromString(string strMsg)
    {
#if NETFX_CORE
        string strAlgName = HashAlgorithmNames.Sha512;

        // Convert the message string to binary data.
        IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(strMsg, BinaryStringEncoding.Utf8);

        // Create a HashAlgorithmProvider object.
        HashAlgorithmProvider hashAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

        // Hash the message.
        IBuffer buffHash = hashAlgProv.HashData(buffUtf8Msg);

        // Verify that the hash length equals the length specified for the algorithm.
        if (buffHash.Length != hashAlgProv.HashLength)
        {
            throw new Exception("There was an error creating the hash");
        }

        // Convert the hash to a string (for display).
        string strHash = cryptoBufferToString(buffHash);

        // Return the encoded string
        return strHash;

#else
        return "";
#endif

    }


    public float[] generateSecretPositionsFromString(string stringToHashFrom, int numPositions)
    {
        int maxElement = 10;
        int scale = 100;
        float multiplier = 2.5f;
        // max element that we return is actually maxElement / scale * multiplier: 0.3 in our case!
        // entropy is maxElement^(2N)
    
        float[] secretPositions = new float[2*numPositions];
        string myHash = generateHashFromString(stringToHashFrom);
        int[] extractedInts = getArrayOfIntsFromHash(myHash, 2 * numPositions, 2 * maxElement + 1);  // for x and y coordinates, between -10 and 10

        for (int i = 0; i < numPositions; ++i)
        {
            secretPositions[2 * i] = (float)(-maxElement + extractedInts[2*i]) / (float) scale * multiplier;
            secretPositions[2*i+1] = (float)(-maxElement + extractedInts[2*i+1]) / (float)scale * multiplier;
        }
        return secretPositions;
    }

    public int[] generateSecretColoringFromString(string stringToHashFrom, int numberOfSecretElements)
    {
        int maxElement = 4;
        string myHash = generateHashFromString(stringToHashFrom);
        // We need twice as many elements: one for color, one for rotation
        return getArrayOfIntsFromHash(myHash, numberOfSecretElements * 2, maxElement);
    }

    // TODO: can we implement this better?
    private int[] getArrayOfIntsFromHash(string myHash, int numberOfRequiredElements, int maxElement)
    {
        Debug.Log("generatedStrongHash: " + myHash);
        // convert stringHash to asciiValues
        byte[] hashBytes = Convert.FromBase64String(myHash);
        int[] result = new int[numberOfRequiredElements];

        long longRunningVal = 0;
        int currentByte = 0;
        for (int i = 0; i < numberOfRequiredElements; ++i)
        {
            if (longRunningVal * (long)256 + hashBytes[currentByte] < (long)Int32.MaxValue)
            {
                longRunningVal = longRunningVal * 256 + hashBytes[currentByte];
                ++currentByte;
            }

            result[i] = (int)longRunningVal % maxElement;
            longRunningVal /= maxElement;
        }

        return result;
    }


// ------------ KEYPAIRS, ENCRYPTION and DECRYPTION ---------------

    public void generateNewKeyPair()
    {
#if NETFX_CORE
        // Create an asymmetric key pair.
        UInt32 keyLength = 512; // This is a must because of the algorithm that we currently use
        myKeyPair = objAlgProv.CreateKeyPair(keyLength);
        // Export the public key to a buffer for use by others.
        IBuffer buffPublicKey = myKeyPair.ExportPublicKey();

        myPublicKeyString = cryptoBufferToString(buffPublicKey);
        Debug.Log("NOVI KLJUC: |" + myPublicKeyString + "|");
#endif
    }

    public string getPublicKey()
    {
        return myPublicKeyString;
    }

#if NETFX_CORE
    // used to convert crypto keys from strings to crypto IBuffers
    private IBuffer cryptoStringToBuffer(string str)
    {
        return CryptographicBuffer.DecodeFromBase64String(str);
    }

    // Used when we want to convert generic strings, which are not crypto keys
    private IBuffer genericStringToBuffer(string str)
    {
        byte[] myBytes = Encoding.UTF8.GetBytes(str);
        return CryptographicBuffer.CreateFromByteArray(myBytes);
    }

    private string cryptoBufferToString(IBuffer buff)
    {
        return CryptographicBuffer.EncodeToBase64String(buff);
    }

    private string genericBufferToString(IBuffer buff)
    {
        return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buff); ;
    }
#endif

    public string encrypt(string plaintext, string publicKeyString)
    {
#if NETFX_CORE
        // Import the public key from a buffer.
        CryptographicKey publicKey = objAlgProv.ImportPublicKey(cryptoStringToBuffer(publicKeyString));

        // Encrypt some data using the public key (not the keypair!).
        IBuffer buffEncryptedData = CryptographicEngine.Encrypt(publicKey, genericStringToBuffer(plaintext), null);

        string encryptedData = cryptoBufferToString(buffEncryptedData);
        return encryptedData;
#else
        return "";
#endif
    }

    public string decryptWithMyKeypair(string encryptedText)
    {
#if NETFX_CORE
        IBuffer buffDecryptedData = CryptographicEngine.Decrypt(myKeyPair, cryptoStringToBuffer(encryptedText), null);
        string decryptedData = genericBufferToString(buffDecryptedData);

        return decryptedData;
#else
        return "";
#endif
    }









    private void TestEncryptDecrypt()
    {
#if NETFX_CORE
        int[] testArray = getArrayOfIntsFromHash(generateHashFromString("MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKY3IxEja9vZf8714tuDSlJzt8KJx74kLZu2mXoHbg50pUy3MS6cEbkNjRbQmkOUpTPaO1Et9rjrmMeHoXSYpIkCAwEAAQ==MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALFespJ+GODzKE+ndLTekpY+rnqCxQzNWMm/HeRrKR0ji3D3XlbUnDUFe1hsfvdOHQVg+aU963g2BeITCs7sKVMCAwEAAQ==Smvq9c8YJDMzCrmqJGfMJDygtvTuXJw3uQ/TCTbHF3E="), 8, 10);


        // Create an asymmetric key pair.
        UInt32 keyLength = 512; // Must be 512 due to the alg. that we are using!
        CryptographicKey keyPair = objAlgProv.CreateKeyPair(keyLength);
        // Export the public key to a buffer for use by others.
        IBuffer buffPublicKey = keyPair.ExportPublicKey();

        generateNewKeyPair();
        generateRandomNonce();

        string tempPub = cryptoBufferToString(buffPublicKey);
        IBuffer reconPubKey = cryptoStringToBuffer(tempPub);
        if (CryptographicBuffer.Compare(reconPubKey, buffPublicKey))
        {
            Debug.Log("Buffer comparison: YES, the same!");
        }
        else
        {
            Debug.Log("Buffer comparison: NO, not the same");
        }

        // Import the public key from a buffer.
        CryptographicKey publicKey = objAlgProv.ImportPublicKey(buffPublicKey);

        string myText = "Moj Testni Text";
        IBuffer myBufferOfText = genericStringToBuffer(myText);

        // Encrypt some data using the public key (not the keypair!).
        IBuffer buffEncryptedData = CryptographicEngine.Encrypt(publicKey, myBufferOfText, null);
        // Debug.Log("ENCR Data: " + buffEncryptedData.ToString());

        string transportString = cryptoBufferToString(buffEncryptedData);

        // -------------------------

        IBuffer buffVal = cryptoStringToBuffer(transportString);

        // Decrypt the data using the private/public keypair (not only private key!!)
        IBuffer buffDecryptedData = CryptographicEngine.Decrypt(keyPair, buffEncryptedData, null);

        string myDecodedString = genericBufferToString(buffDecryptedData);

        // Debug.Log("DECR Data: " + myDecodedString);
#endif
    }

    // Use this for initialization
    void Start()
    {
#if NETFX_CORE
        objAlgProv = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
        // objKdfProv = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256); // uncomment if we start using symmetric key derivation from shared secret
#endif
//        TestEncryptDecrypt();
    }

    // Update is called once per frame
    void Update()
    {

    }
}














//// Ovo nam trenutno ne treba, ali cemo u buducnosti mozda htjeti na dobar nacin generirati simetricni kljuc iz zajednickog secreta, pa ostavljam u kodu

//public string deriveSymmetricKeyfromString(string sharedKeyMaterial)
//{
//// Derive key material from a password-based key derivation function.
//UInt32 targetKeySize = 32;
//UInt32 iterationCount = 10000;
//IBuffer buffKeyMatl = deriveKeyMaterialPbkdf(sharedKeyMaterial);

//// Create a key.
//CryptographicKey key = objKdfProv.CreateKey(buffKeyMatl);
//// Create a key by using the KDF parameters.

//IBuffer keyBuffer = key.ExportPublicKey();

//    return cryptoBufferToString(keyBuffer);
//}

//// We need this function to ensure that the key derivation is indeed secure. More info: https://en.wikipedia.org/wiki/HKDF
//// Source: https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.keyderivationalgorithmprovider
//public IBuffer deriveKeyMaterialPbkdf(string strSecret)
//{
//// Demonstrate how to retrieve the algorithm name.
//string strAlgUsed = objKdfProv.AlgorithmName;

//// Create a buffer that contains the secret used during derivation.
//IBuffer buffSecret = CryptographicBuffer.ConvertStringToBinary(strSecret, BinaryStringEncoding.Utf8);

//// Create a random salt value.
//IBuffer buffSalt = CryptographicBuffer.GenerateRandom(32);

//// Create the derivation parameters.
//KeyDerivationParameters pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, 10000);

//// Create a key from the secret value.
//CryptographicKey keyOriginal = objKdfProv.CreateKey(buffSecret);

//// Derive a key based on the original key and the derivation parameters.
//IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Params, 32);

//// Demonstrate checking the iteration count.
//UInt32 iterationCountOut = pbkdf2Params.IterationCount;

//// Demonstrate returning the derivation parameters to a buffer.
//IBuffer buffParams = pbkdf2Params.KdfGenericBinary;

//    // return the KDF key material.
//    return keyMaterial;
//}
