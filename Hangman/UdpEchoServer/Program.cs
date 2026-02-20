using System.Net; // For IPAddress
using System.Net.Sockets; // For UdpClient
using System.Text;

class UdpServer {
	static string correctWord;
	static HashSet<char> guessedChars = new();

	static void Main() {
		correctWord = "heksenkaas";
		StartServer(50001);
	}

	static void StartServer(int port) {
		// Create a UdpClient on the given port.
		// Note that in UDP, since it's not connection based,
		// there is not much difference between clients and servers.
		// However, a UdpClient that plays the role of server should have a known port, such that
		//  other clients can reach it:
		UdpClient client = new UdpClient(port);
		Console.WriteLine($"Starting UDP server on port {port} - listening for incoming messages");

		IPEndPoint remote = new IPEndPoint(IPAddress.Any,0);
		while (true) {
			// The next call blocks until a packet comes in.
			// Packets can be received from anywhere!
			// The [remote] ref (=output parameter) tells us where this packet came from:
			byte[] packet = client.Receive(ref remote);

            string request = Encoding.ASCII.GetString(packet, 0, packet.Length);
			request = "\n" + ProcessRequest(request);

			byte[] newPacket = Encoding.ASCII.GetBytes(request);

            Console.WriteLine($"Received a {packet.Length} packet from {remote}. Echoing...");
			// Don't do anything with the packet, just send it back to where it came from: 
			client.Send(newPacket, remote);
		}
	}

    static string ProcessRequest(string request)
	{
		if (request.Length == 1) // Single character
		{
			char character = request[0];
			if (guessedChars.Add(character))
			{
				if (correctWord.Contains(character)) return "Character is in the target word\n" + GetCurrentWordState();
				else return "Character is not in the target word\n" + GetCurrentWordState();
			}
			else return "Already guessed this character, try a different one.\n" + GetCurrentWordState();
			
		}
		else if (request.Length == correctWord.Length) // Full word
		{
			if (correctWord == request) return "Guessed correctly!\n";
			else return "Wrong guess.\n";
		}
		else // Invalid guess
		{
			return "Invalid length. Enter a single character, or enter a word with the same length as the target word\n";
		}
	}

	static string GetCurrentWordState()
	{
		string result = string.Empty;
		for (int i = 0; i < correctWord.Length; i++)
		{
			char character = correctWord[i];
			if (guessedChars.Contains(character)) result += character;
			else result += '-';
		}
		return result + "\n";
	}
}

