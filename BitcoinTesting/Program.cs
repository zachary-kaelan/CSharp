using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Stealth;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace BitcoinTesting
{
    class Program
    {
        public static readonly Network NETWORK = Network.TestNet;
        public static QBitNinjaClient client = new QBitNinjaClient(Network.Main);
        static void Main(string[] args)
        {

            
            // ---------------------------------------------------------

            // You only have to share one address with the world (called StealthAddress) without leaking any privacy.
            // If you share one BitcoinAddress with everybody, then all can see your balance by consulting the blockchain.
            // A better name than "dark" would've been "One Address".
            // The Payer can use the StealthAddress to generate many new addresses.
            // All the coins distributed among these throwaway addresses will be spendable by the Receiver.
            // Only the Payer and the Receiver knows that the addresses generated are related, a third party investigating the blockchain doesn't.

            // The Payer knows the StealthAddress of the Receiver.
            // The Receiver knows the Spend Key, a secret that will allow him to spend the coins received.
            // The Scanner knows the Scan key, a secret that allows him to detect the transactions that belong to the Receiver.

            // Underneath, the StealthAddress is composed of one or several SpendPubKeys and one ScanPubKey.

            var scanKey = new Key();
            var spendKey = new Key();
            BitcoinStealthAddress stealthAddress = new BitcoinStealthAddress(
                scanKey.PubKey,
                new[] { spendKey.PubKey },
                signatureCount: 1,
                bitfield: null,
                network: Network.Main
            );

            Console.ReadLine();
        }

        public static void Keys()
        {
            Key privateKey = new Key(); // Used to spend money from your wallet
            PubKey publicKey = privateKey.PubKey;
            BitcoinPubKeyAddress address = publicKey.GetAddress(Network.Main); // Combination of public key and network
            // Bitcoin address = version byte + public key hash bytes
            var publicKeyHash = publicKey.Hash; // public key hash = RIPEMD160(SHA256(pubkey))
            var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
            var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);

            // To the Blockchain, addresses don't exist. Just a ScriptPubKey
            // OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
            // Short script explaining conditions needed to claim bitcoin ownersship
            var scriptPublicKey = testNetAddress.ScriptPubKey;
            // ScriptPubKey is the same for both networks.
            // Also contains the hash of the public key

            BitcoinSecret testNetPrivateKey = privateKey.GetBitcoinSecret(Network.TestNet);
            // Secret is the private version of the Bitcoin Address
            // Also know as Wallet Import Format (WIF)
            Key samePrivateKey = testNetPrivateKey.PrivateKey;
            // You can get the Private Key from the BitcoinSecret easily
            // However, you cannot get the Public Key from the Bitcoin Address because the Address contains only a hash of it
        }

        public static void Transactions()
        {
            // The blockchain is a global ledger of transactions
            // A transaction may have no recipient, or it may have several. Same for senders.
            // Sender and recipient are always abstracted with a ScriptPubKey
            // Transaction ID = SHA256(SHA256(txbytes))
            // Transaction IDs can be manipulated before they are confirmed

            // You can use QBitninja.Client to view the raw bytes of transactions by ID
            //Transaction tx = new Transaction("bytes"); // can parse the transaction from hex

            var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
            GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
            // Convert to NBitcoin type
            Transaction transaction = transactionResponse.Transaction;
            // The transaction response as additional important information like the value and scriptPubKey of the inputs

            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            foreach (var coin in receivedCoins)
            {
                Money amount = (Money)coin.Amount;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = coin.TxOut.ScriptPubKey;
                Console.WriteLine(paymentScript);
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
                Console.WriteLine();
            }
            // For NBitcoin's Transaction class
            var outputs = transaction.Outputs;
            foreach (TxOut output in outputs)
            {
                Money amount = output.Value;
                // etc...
            }
            // inputs reference previous ouputs that have been spent
            var inputs = transaction.Inputs;
            foreach (TxIn input in inputs)
            {
                OutPoint previousOutpoint = input.PrevOut;
                Console.WriteLine(previousOutpoint.Hash); // hash of previous transaction
                Console.WriteLine(previousOutpoint.N); // idx of out from previous transaction, that has been spent in the currrent transaction
            }
            // TxOut, Output, and out are synonymous
            // TxOut, in summary, represents an amount of bitcoin and a ScriptPubKey

            // Outpoint = Transaction ID + Index of TxOut
            // TxIn = Outpoint + ScriptSig

            OutPoint firstPreviousOutPoint = transaction.Inputs.First().PrevOut;
            var firstPreviousTransaction = client.GetTransaction(firstPreviousOutPoint.Hash).Result.Transaction;
            Console.WriteLine(firstPreviousTransaction.IsCoinBase);
            // You can continue to trace transactions IDs back until a coinbase transaction is reached
            // A coinbase transaction includes the newly mined coin by a miner.

            // 0.0002 BTC are not accounted for
            // This is the fee collected by the miner for including the transaction in a block
            var fee = transaction.GetFee(transactionResponse.SpentCoins.ToArray());
            // a coinbase transaction is the only transaction whose value of output are superior to value of input
            // effectively corresponds to coin creation
            // no fee in a coinbase transaction, the first transaction of every block
            // Consensus rules enforce that the sum of output's value in the coinbase transaction does not exceed the sum of transaction fees in the block plus the mining reward
        }
        
        public static void BeautyOfTheBlockchain()
        {
            // While the ownership of the spent TxOut has been proved, it has not yet been proven that it actually exists.
            // The database of all transactions is duplicated all around the world.
            // Once a transaction appears on the Blockchain, it is very easy to prove its existence.

            // Miners have one goal: inserting transactions in the Blockchain
            // However, instead of modifying the blockchain with every single transaction, they are added in a batch or "block".
            // Other nodes in the network confirm the new block obeys the rules set forth in the Bitcoin protocol.
            // If two miners add a block at the same time, there is a fork.
            // Ultimately, only the branch of the fork with the most work will be continued.
            // If a miner tries to include an invalid transaction in a block, other nodes will not recognize it and the miner loses that block's investment.

            // Once a valid block is submitted, all transactions inside are considered Confirmed.
            // When this happens, all miners must discard current work and begin working on a new block using new transactions.
            // Once confirmed a block is added to the Blockchain. Likelihood of being undone decreases dramatically as other blocks stack on top.

            // You can verify transactions by either checking the entire Blockchain, or asking for a pratial Merkle tree.

            // Regardless on the viability of Bitcoin, the fortress-like security that a Blockchain provides has tremendous implications.
            // Notaries who record facts in court could store documents permanently in the Blockchain.
            // Audits can become automatic and provable when assets and ownership are stored and transferred on the Blockchain.
            // Automatic trading scripts can trade between themselves without human intervention or authorization.
        }

        public static void SpendingYourCoins()
        {
            var privateKey = new Key();
            var bitcoinPrivateKey = privateKey.GetWif(NETWORK);
            var address = bitcoinPrivateKey.GetAddress();
            Console.WriteLine(bitcoinPrivateKey);
            Console.WriteLine(address);
            Console.WriteLine();

            var client = new QBitNinjaClient(NETWORK);
            var transactionId = uint256.Parse("cfb97ef3257af7251cd5fe41d264e732bf1299630943fcf904fc43d9c7c5b413");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            Console.WriteLine(transactionResponse.TransactionId);
            //Console.WriteLine(transactionResponse.Block.Confirmations);

            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = receivedCoins.Last(c => c.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey).Outpoint;
            Console.WriteLine(outPointToSpend == null ? "TxOut doesn't contain our ScriptPubKey." : "We want to spend " + (outPointToSpend.N + 1).ToString() + ". outpoint:");

            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn() { PrevOut = outPointToSpend });
            // You need to reference the outpoint to spend the coins

            var hallOfTheMakersAddress = BitcoinAddress.Create("mzp4No5cmCXjZUpf112B1XWsvWBfws5bbB", Network.TestNet);
            // book challenge address
            // 1 bitcoin = 1000000 bits
            // 1 bit = 100 stoshis

            // An unspent output holds 0.001 BTC, so to donate 0.0004 BTC you need to send all of it.
            // 0.0004 BTC goes to Hall of the Makers, 0.00053 BTC goes to you, and 0.00007 BTC goes to the miner.
            // This fee incentivizes the miners to add this transaction to their next block.
            // If you set the miner fee to zero, your transaction might never be confirmed.

            // Add a message (must be <= 80 bytes)
            var message = "Long live NBitcoin and its makers!";
            var bytes = Encoding.UTF8.GetBytes(message);
            transaction.Outputs.Add(
                new TxOut()
                {
                    Value = Money.Zero,
                    ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
                }
            );

            // Signing
            transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
            transaction.Sign(bitcoinPrivateKey, false);

            //Sending it with QBitNinja
            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaction in any block explorer:");
                Console.WriteLine(transaction.GetHash());
            }
        }

        public static void ProvingYouOwnAnAddress()
        {
            // the first ever Bitcoin transaction
            /*var bitcoinPrivateKey = new BitcoinSecret("XXXXXXXXXXXXXXXXXXXXXXXXXX");

            var message = "I am Craig Wright";
            string signature = bitcoinPrivateKey.PrivateKey.SignMessage(message);
            Console.WriteLine(signature); // IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=*/

            // let's verify it
            var message = "I am Craig Wright";
            var signature = "IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=";

            var address = new BitcoinPubKeyAddress("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");
            bool isCraigWrightSatoshi = address.VerifyMessage(message, signature);

            Console.WriteLine("Is Craig Wright Satoshi? " + isCraigWrightSatoshi);
            // the bool is false, Craig was using social engineering
            // You prove you are the owner of an addess without moving coins by giving out the following:

            // Address: 1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB
            // Message: Nicolas Dorier Book Funding Address
            // Signature: H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=
        }

        public static void CreatingAMoreRandomKey()
        {
            // Uses the RNGCryptoServiceProvider to generate private keys.
            // Malware can modify your PRNG and predict the numbers generated.
            // Most PRNG uses a Seed then generates a series of random numbers from it.
            // The amount of randomness of the seed is defined by a measure called Entropy.
            // The amount of Entropy also depends on the observer.

            // Say you generate a seed from your clock time.
            // Assuming that your clock has a resolution of 1ms (really more ~15ms).
            // If your attacker knows that you generated the key last week...
            // ... your seed has 1000 * 60 * 60 * 24 * 7 = 604800000 possibilities
            // The Entropy for the attacker is log2(604800000) = 29.17 bits.
            // Enumerating such a number on a home computer takes less than 2 seconds.

            // Let's say you use the clock time + process id for generating the seed.
            // There are 1024 different process ids
            // Now there are 604800000 * 1024 possibilies, which takes around 2000 seconds to enumerate.
            // Now let's add the time when I turned on my computer today, assuming the attack knows I turned it on today.
            // Now it is 604800000 * 1024 * 86400000 = 5.35088E+19 possibilities.
            // Though if the attacker has infiltrated your computer they can get that last piece of info and reduce entropy.
            // Entropy = log2(possibilities) so the Entropy of this new number is 65 bits.

            // The hash of a public key is 20 bytes (160 bits), it is smaller than the total universe of the addresses.
            // You might do better.
            // Note: Adding entropy is linearly harder, cracking entropy is exponentially harder.
            // An interesting way of generating entropy quickly is by incorporating human intervention, such as moving the mouse.
            // If you don't completely trust platform PRNG, you can add entropy to the PRNG output used by NBitcoin.

            RandomUtils.AddEntropy("hello");
            RandomUtils.AddEntropy(new byte[] { 1, 2, 3 });
            var nsaProofKey = new Key();

            // additionalEntropy = SHA(SHA(data) ^ additionalEntropy)
            // result = (SHA(PRNG() ^ additionalEntropy)
        }

        public static void KeyDerivationFunction()
        {
            // KDF or Key Derivation Function is a way to have a stronger key, despite low entropy.
            // A KDF is a hash function that wastes computing resources on purpose.

            // var derived = SCrypt.BitcoinComputeDerivedKey("hello", new byte[] { 1, 2, 3 });
            // RandomUtils.AddEntropy(derived);
            // Even if the attack knows that the source of entropy is 5 letters, he needs to run SCrypt to check each possibility, taking 5 seconds.

            // A standard already exists for encrypting your private key with a password using a KDF. This is BIP38

            var privateKey = new Key();
            var bitcoinPrivateKey = privateKey.GetWif(Network.Main);
            Console.WriteLine(bitcoinPrivateKey);
            BitcoinEncryptedSecret encryptedBitcoinPrivateKey = bitcoinPrivateKey.Encrypt("password");
            Console.WriteLine(encryptedBitcoinPrivateKey);
            var decryptedBitcoinPrivateKey = encryptedBitcoinPrivateKey.GetSecret("password");
            Console.WriteLine(decryptedBitcoinPrivateKey);

            // Why generate several keys?
            // The main reason is privacy.
            // You can see the balance of all Bitcoin addresses.
            // It is better to use a new address for each transaction.
            // In practice, you can also generate keys for each contact which makes this a simple way to identify your payer without leaking too much privacy.
            // However, all bakcups of your wallet that you have will become outdated when you generate a new key.
            // You cannot delegate the address creation process to an untrusted peer.
        }

        public static void BIP38PassphraseCode()
        {

            // BIP is in reality two ideas in one document.
            // The second part of the BIP lets you delegate Key and Address creation to an untrusted peer.
            // ---
            // You generate a PassphraseCode to the key generator.
            // With this PassphraseCode, they will be able to generate encrypted keys on your behalf.
            // They don't have to know your password or any private key.
            // The PassphraseCode can be given to your key generator in WIF format.
            // In NBitcoin, all types prefixed by "Bitcoin" are Base58 (WIF) data.

            var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);
            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();
            var generatedAddress = encryptedKeyResult.GeneratedAddress;
            var encryptedKey = encryptedKeyResult.EncryptedKey;
            var confirmedCode = encryptedKeyResult.ConfirmationCode;

            // The ConfirmationCode lets the third party prove that the generated key and address correspond to your password.
            // Now, as the owner...

            // Check to make sure the generator didn't cheat.
            Console.WriteLine(confirmedCode.Check("my secret", generatedAddress));
            var bitcoinPrivateKey = encryptedKey.GetSecret("my secret");
            Console.WriteLine(bitcoinPrivateKey.GetAddress() == generatedAddress);
            Console.WriteLine(bitcoinPrivateKey);

            // BIP 32, or Hierarchical Deterministic Wallets (HD wallets) proposes a solution to prevented outdated backups.
        }

        public static void HDWallets()
        {
            // Deterministic wallet means you only have to save the seed.
            // From the seed, you can generate the same series of private keys over and over.

            ExtKey masterKey = new ExtKey();
            Console.WriteLine("Master key : " + masterKey.ToString(Network.Main));
            for (int i = 0; i < 5; ++i)
            {
                ExtKey key = masterKey.Derive((uint)i);
                Console.WriteLine("Key " + i + " : " + key.ToString(Network.Main));
            }

            // You can go back from a Key to an ExtKey by supplying the Key and the ChainCode to the ExtKey constructor.

            ExtKey extKey = new ExtKey();
            byte[] chainCode = extKey.ChainCode;
            Key key = extKey.PrivateKey;
            ExtKey newExtKey = new ExtKey(key, chainCode);

            // BitcoinExtKey is a base58 type equivalent
            // You can get a Public version of the master key and generate public keys without knowing the private.

            ExtPubKey masterPubKey = masterKey.Neuter();
            for (int i = 0; i < 5; ++i)
            {
                ExtPubKey pubKey = masterPubKey.Derive((uint)i);
                Console.WriteLine("PubKey " + i + " : " + pubKey.ToString(Network.Main));
            }

            // Then you can get the corresponding private keys with the private master key.
            ExtPubKey pubKey1 = masterPubKey.Derive((uint)1);
            ExtKey key1 = masterKey.Derive((uint)1);

            // Check its legitimacy
            Console.WriteLine("Generated address : " + pubKey1.PubKey.GetAddress(Network.Main));
            Console.WriteLine("Expected address : " + key1.PrivateKey.PubKey.GetAddress(Network.Main));

            // However, the hierarchical side of the wallet comes in when you start deriving grandchild keys from the child keys.
            ExtKey key12 = key1.Derive((uint)2);
            // Or
            ExtKey key11 = masterKey.Derive(new KeyPath("1/1"));

            // Hierarchical keys are a nice way to classify the type of your keys for multiple accounts.
            // You can also securely separate off different people within these specific classifications.
            // However, you can normally get the master private key from the master public key and child private key

            ExtKey ceoKey = new ExtKey();
            Console.WriteLine("CEO: " + ceoKey.ToString(Network.Main));

            ExtKey accountingKey = ceoKey.Derive(0, false);
            ExtPubKey ceoPubKey = ceoKey.Neuter();

            ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubKey);
            Console.WriteLine("CEO recovered: " + ceoKeyRecovered.ToString(Network.Main));

            // In other words, it's a 2-way path for non-hardened keys.
            // Non-hardened keys should only be used for categorizing accounts that belong to a point of single control.
            // If you try to recover a parent's private key with a hardened child key, the program will crash.
            // You can also harden a childKey with the KeyPath by using an apostrophe after the child's index.

            var nonHardened = new KeyPath("1/2/3");
            var hardened = new KeyPath("1/2/3'");

            // Let's imagine that Accounting generates 1 parent key for each customer and a child for each payment.

            string accounting = "1'";
            int custoemrId = 5;
            int paymentId = 50;
            KeyPath path = new KeyPath(accounting + "/" + custoemrId + "/" + paymentId);
            // Path: "1'/5/50"
            ExtKey paymentKey = ceoKey.Derive(path);

            // Cold wallets like Trezor generator the HD Keys from a sentence that can easily be written down.
            // This sentence is referred to as "the seed" or "mnemonic".
            // It can eventually be protected by a password or a PIN.
            // The language that you use to generate your 'easy to write' sentence is called a Wordlist.

            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot = mnemo.DeriveExtKey("my password");
            Console.WriteLine(mnemo);
            Mnemonic mnemoRecovered = new Mnemonic("minute put grant neglect anxiety case globe win famous correct turn link", Wordlist.English);
            ExtKey hdRootRecovered = mnemo.DeriveExtKey("my password");
        }
    }
}
