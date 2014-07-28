package Helpers;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.Set;
import edu.stanford.nlp.dcoref.CorefChain;
import edu.stanford.nlp.dcoref.CorefChain.CorefMention;
import edu.stanford.nlp.dcoref.CorefCoreAnnotations.CorefChainAnnotation;
import edu.stanford.nlp.ling.CoreAnnotations;
import edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation;
import edu.stanford.nlp.ling.CoreLabel;
import edu.stanford.nlp.ling.CoreAnnotations.TextAnnotation;
import edu.stanford.nlp.ling.CoreAnnotations.TokensAnnotation;
import edu.stanford.nlp.ling.HasWord;
import edu.stanford.nlp.ling.TaggedWord;
import edu.stanford.nlp.parser.lexparser.LexicalizedParser;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.pipeline.StanfordCoreNLP;
import edu.stanford.nlp.process.Morphology;
import edu.stanford.nlp.trees.PennTreebankLanguagePack;
import edu.stanford.nlp.trees.Tree;
import edu.stanford.nlp.trees.TreebankLanguagePack;
import edu.stanford.nlp.util.CoreMap;
import edu.stanford.nlp.util.IntPair;

public class StanfordCoreNLPHelper 
{
	private static LexicalizedParser lexicalizedParser;
	private static StanfordCoreNLP pipeline;
	private static Boolean lexicalizedParserInitialized = false;
	private static Boolean pipelineInitialized = false;
	public static void initializeLexicalizedParser()
	{
		if (!lexicalizedParserInitialized)
		{
			lexicalizedParser = LexicalizedParser.loadModel("edu/stanford/nlp/models/lexparser/englishPCFG.ser.gz");
		    lexicalizedParserInitialized = true;
		}
	}
	public static void initializePipeline()
	{
		if (!pipelineInitialized)
		{
		    Properties props = new Properties();
		    props.put("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
		    props.put("tokenize.options","ptb3Escaping=false"); // disable traditional PTB3 token transforms (like parentheses becoming -LRB-, -RRB-)
		    pipeline = new StanfordCoreNLP(props);
		    pipelineInitialized = true;
		}
	}
	public static Annotation getAnnotation(String text)
	{
		initializePipeline();
	    Annotation annotation = new Annotation(text);
	    pipeline.annotate(annotation); 
	    return annotation;
	}
	public static String getLemma(String word)
	{
		Morphology m = new Morphology();		
	    return m.stem(word);
	}
	public static String getCoreferencedText(Annotation annotation)
	{
		String result = "";
		String token = null;
		String sourceToken = null;	
		List<CoreMap> sentences = annotation.get(CoreAnnotations.SentencesAnnotation.class); 
	    for(int s = 0; s < sentences.size(); s++) 
	    {
	    	List<CoreLabel> sentence = sentences.get(s).get(TokensAnnotation.class);
	    	for (int t = 0; t < sentence.size(); t++) 
		    {
	    		sourceToken = StanfordCoreNLPHelper.getSourceToken(annotation, new IntPair(s + 1, t + 1));
	    		if (sourceToken != null)
	    		{
	    			token = sourceToken;
	    		}
	    		else
	    		{
	    			token = sentence.get(t).get(TextAnnotation.class);
	    		}
	    		result += token + " ";
		    }
	    }
	    return result; 
	}
	
	private static String getSourceToken(Annotation annotation, IntPair key)
	{
		String sourceToken = null;
		
		// get the source token only for PRP & PRP$ 
		int tokenSentenceIndex = key.getSource() - 1;
		int tokenIndex =  key.getTarget() - 1;
		String tokenPOS = annotation.get(CoreAnnotations.SentencesAnnotation.class).get(tokenSentenceIndex).get(TokensAnnotation.class).get(tokenIndex).get(PartOfSpeechAnnotation.class);		
		if (!tokenPOS.equals("PRP$") & !tokenPOS.equals("PRP"))
		{
			return null;
		}
		// end
		
		Map<Integer, CorefChain> graph =  annotation.get(CorefChainAnnotation.class);
		IntPair sourceKey = null;
		for (CorefChain c : graph.values())
	    {
	    	Map<IntPair, Set<CorefMention>> chain = c.getMentionMap();
	    	if (chain.size() > 1)
	    	{ 	
	    		List<IntPair> chainOfKeys = new ArrayList<IntPair>(chain.keySet());
	    		Collections.sort(chainOfKeys);
	    		sourceKey = chainOfKeys.get(0);
	    		for (IntPair p : chainOfKeys)
	    		{			
	    			if (p.equals(key))
	    			{	 
	    				int s = sourceKey.getSource() - 1;
    					int t = sourceKey.getTarget() - 1;
    					sourceToken = annotation.get(CoreAnnotations.SentencesAnnotation.class).get(s).get(TokensAnnotation.class).get(t).get(TextAnnotation.class);
	    				break;
	    			}	
	    		}	    		
	    	}
	    }	
		
		
		if (sourceToken != null)
		{
			//if the source token is preposition also don't return it
			int s = sourceKey.getSource() - 1;
			int t = sourceKey.getTarget() - 1;
			String sourceTokenPOS = annotation.get(CoreAnnotations.SentencesAnnotation.class).get(s).get(TokensAnnotation.class).get(t).get(PartOfSpeechAnnotation.class);		
			if (sourceTokenPOS.equals("PRP$") || sourceTokenPOS.equals("PRP"))
			{
				return null;
			}
			// end
			
			// add 's to position prepositions like: his latptop -> Bilal's laptop
			if (tokenPOS.equals("PRP$")) 
			{
				sourceToken += "'s";
			}
			//end
		}		
		
		return sourceToken;
	}	
	
	public static String cleanPredicate(String pr)
	{
		initializeLexicalizedParser();
		String result = null;
		String sent = pr;
		TreebankLanguagePack tlp = new PennTreebankLanguagePack();
		List<? extends HasWord> tokens = tlp.getTokenizerFactory().getTokenizer(new StringReader(sent)).tokenize(); 
		Tree parse = lexicalizedParser.apply(tokens);
		List<TaggedWord> taggedWords = parse.taggedYield();
		int lastTokenIndex = taggedWords.size() - 1;
		TaggedWord lastToken = taggedWords.get(lastTokenIndex);
		if (lastToken.tag().equals("IN"))
		{
			lastTokenIndex--;
		}
		result = taggedWords.get(lastTokenIndex).word();
		return result;
	}
}
