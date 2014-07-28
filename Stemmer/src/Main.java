import edu.stanford.nlp.process.Morphology;

public class Main 
{
	public static void main(String[] args) 
	{
		String input ="providers played";
		String output = "";
		Morphology m = new Morphology();
		if (args.length > 0 )
		{
			input = args[0];
		}
		String[] words = input.split(" ");
		for (int i = 0; i < words.length; i++)
		{
			output += m.stem(words[i]) + ";";
		}
		System.out.print(output);
	}
}
