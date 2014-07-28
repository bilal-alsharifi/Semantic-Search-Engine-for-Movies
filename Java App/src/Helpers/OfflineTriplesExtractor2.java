package Helpers;
import java.util.ArrayList;
import java.util.List;
import java.io.StringReader;
import Entities.Sentence;
import Entities.Triple;
import edu.stanford.nlp.objectbank.TokenizerFactory;
import edu.stanford.nlp.process.CoreLabelTokenFactory;
import edu.stanford.nlp.process.PTBTokenizer;
import edu.stanford.nlp.ling.CoreLabel;
import edu.stanford.nlp.trees.*;
import edu.stanford.nlp.parser.lexparser.LexicalizedParser;

public class OfflineTriplesExtractor2 {
	private static LexicalizedParser lp;
	private static boolean initialized = false;
	  private static  List<Triple> Formater(String sceneTime, List<TypedDependency> td)
	  {
		  ArrayList<Triple> triples = new ArrayList<>();
		  ArrayList<Sentence> sl = new ArrayList<>();
		  for (TypedDependency typedDependency : td) {
			  if (typedDependency.reln().toString().equals("nsubj"))
			  {
				  sl.add(new Sentence(typedDependency.dep().toString(), typedDependency.gov().toString()));
			  }
			  else if (typedDependency.reln().toString().equals("rcmod"))
			  {
				 sl.add(new Sentence( typedDependency.gov().toString(), typedDependency.dep().toString()));
			  }
			  else if (typedDependency.reln().toString().equals("dobj"))
			  {
				  for (Sentence sentence : sl) {
					if (sentence.getAction().equals(typedDependency.gov().toString()))
					{
						String S1 = sentence.getSubject().substring(0, sentence.getSubject().lastIndexOf('-'));
						String S2 = sentence.getAction().substring(0, sentence.getAction().lastIndexOf('-'));
						String S3 = typedDependency.dep().toString().substring(0, typedDependency.dep().toString().lastIndexOf('-'));
						triples.add(new Triple(sceneTime, S1, S2, S3, false));
					}
				}
			  }
			  else if (typedDependency.reln().toString().contains("prep_"))
			  {
				  //String sentence = "prep_";
				  //sentence = typedDependency.reln().toString().substring(sentence.length());
				  for (Sentence sentence : sl) {
						if (sentence.getAction().equals(typedDependency.gov().toString()))
						{	
							String S1 = sentence.getSubject().substring(0, sentence.getSubject().lastIndexOf('-'));
							String S2 = sentence.getAction().substring(0, sentence.getAction().lastIndexOf('-'));
							String S3 = typedDependency.dep().toString().substring(0, typedDependency.dep().toString().lastIndexOf('-'));
							triples.add(new Triple(sceneTime, S1, S2, S3, false));
						}
					}
			  }
			  else if (typedDependency.reln().toString().equals("cop"))
			  {
				  for (Sentence sentence : sl) {
						if (sentence.getAction().equals(typedDependency.gov().toString()))
						{
							String S1 = sentence.getSubject().substring(0, sentence.getSubject().lastIndexOf('-'));
							String S2 = typedDependency.dep().toString().substring(0, typedDependency.dep().toString().lastIndexOf('-'));
							String S3 = sentence.getAction().substring(0, sentence.getAction().lastIndexOf('-'));
							triples.add(new Triple(sceneTime, S1, S2, S3, false));
						}
					}
			  }
			  else if (typedDependency.reln().toString().equals("nsubjpass"))
			  {
				  sl.add(new Sentence(typedDependency.dep().toString(), typedDependency.gov().toString()));
			  }
			  else if (typedDependency.reln().toString().equals("auxpass"))
			  {

				  for (Sentence sentence : sl) {
						if (sentence.getAction().equals(typedDependency.gov().toString()))
						{
							String S1 = sentence.getSubject().substring(0, sentence.getSubject().lastIndexOf('-'));
							String S2 = sentence.getAction().substring(0, sentence.getAction().lastIndexOf('-'));
							String S3 = typedDependency.dep().toString().substring(0, typedDependency.dep().toString().lastIndexOf('-'));
							triples.add(new Triple(sceneTime, S1, S2, S3, false));
						}
					}
			  }
		}
		  return triples;
	  }
  //public List<TypedDependency> Analyze(LexicalizedParser lp, String S) {
  public static List<Triple> getTriples(String sceneTime, String text ) {
	  initialize();
  // This option shows loading and using an explicit tokenizer
    TokenizerFactory<CoreLabel> tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
    List<CoreLabel> rawWords2 = tokenizerFactory.getTokenizer(new StringReader(text)).tokenize();
    Tree parse = lp.apply(rawWords2);
    TreebankLanguagePack tlp = new PennTreebankLanguagePack();
    GrammaticalStructureFactory gsf = tlp.grammaticalStructureFactory();
    GrammaticalStructure gs = gsf.newGrammaticalStructure(parse);
    List<TypedDependency> tdl = gs.typedDependenciesCCprocessed();
    return Formater(sceneTime, tdl);
    //return tdl;
}

  public OfflineTriplesExtractor2() {
	  
  } 

  private static void initialize()
  {
	  if (!initialized)
	  {
		  lp = LexicalizedParser.loadModel("edu/stanford/nlp/models/lexparser/englishPCFG.ser.gz");
		  initialized = true;
	  }
  }

}
