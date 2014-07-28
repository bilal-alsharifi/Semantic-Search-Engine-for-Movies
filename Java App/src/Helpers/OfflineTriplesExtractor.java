package Helpers;

import edu.stanford.nlp.ling.CoreAnnotations;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.pipeline.StanfordCoreNLP;
import edu.stanford.nlp.trees.*;
import edu.stanford.nlp.trees.TreeCoreAnnotations.TreeAnnotation;
import edu.stanford.nlp.util.CoreMap;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Properties;

import Entities.Triple;
/**
 *
 * @author Anna Adjemian
 */
public class OfflineTriplesExtractor {

    public String Subject = "";
    public String Predicate = "";
    public List<String> Objects;
    int maxDepth = 1;
    Tree PredicateTree;
    static List<String> Nouns;
    static List<String> Verbs;
    static List<String> Adjectives;
    
    
    
    // added by Bilal
	private static StanfordCoreNLP pipeline;
    static boolean initialized = false;

    
    public OfflineTriplesExtractor()
    {
    	initialize();
    }
    
	public static void initialize()
	{
		if (!initialized)
		{
		    Properties props = new Properties();
	        props.put("annotators", "tokenize, ssplit, parse");
		    pipeline = new StanfordCoreNLP(props);
		    setLists();
		    initialized = true;
		}
	}
	
	public static List<Triple> getTriples(String sceneTime, String text)
	{
		initialize();
		List<Triple> tripples = new ArrayList<Triple>();
        Annotation annotation = new Annotation(text);
        pipeline.annotate(annotation);
        List<CoreMap> sentences = annotation.get(CoreAnnotations.SentencesAnnotation.class);
        for(CoreMap sentence: sentences)
        {
        	OfflineTriplesExtractor triple = new OfflineTriplesExtractor();          
            Tree parse = sentence.get(TreeAnnotation.class);
            if (triple.Extract_Triples(parse.getChild(0))) 
            {
                for (int i =0;i< triple.Objects.size(); i++)
                {
                	tripples.add(new Triple(sceneTime, triple.Subject, triple.Predicate, triple.Objects.get(0), false));
                }
            }               
        }
        return tripples;
	}
	//----------------------------------------
	
	
  public static void setLists ()
  {
      Nouns = new LinkedList<String>();
      Verbs = new LinkedList<String>();
      Adjectives = new LinkedList<String>();
      Nouns.add("NN");  Nouns.add("NNP");  Nouns.add("NNPS");   Nouns.add("NNS");
      Verbs.add("VB");  Verbs.add("VBD");  Verbs.add("VBG");   Verbs.add("VBP");   Verbs.add("VBN");   Verbs.add("VBZ");
      Adjectives.add("JJ");  Adjectives.add("JJR");  Adjectives.add("JJS");
      
  }
  public int breadthFirst_Search (Tree[] childs, List<String> S)
  {
    boolean NPfound = false;
    int id = 0;

    while ((!NPfound) && (id < childs.length))
    {
        if (S.contains(childs[id].label().value()))
            NPfound = true;
        else
            id++;
    }
    if (NPfound)
        return id;
    else
        return -1;
  }
  public void breadthFirst_Search2 (Tree currentTree, List<String> verbs,Tree parseTree)
  {
      Tree[] childs = currentTree.children();
      int i = 0;
      while (i < childs.length)
      {
          if (verbs.contains(childs[i].label().value()))
          {
              if ((parseTree.depth(childs[i])) >  maxDepth)
              {
                  maxDepth = parseTree.depth(childs[i]);
                  Predicate = childs[i].firstChild().label().value();
                  PredicateTree = childs[i];
              }
          }
          else if (childs[i].label().value().equals("VP"))
          {
               breadthFirst_Search2(childs[i], verbs, parseTree);
          }
          i++;
      }
  }


  public void Extract_Subject (Tree parseTree)
  {
      List<String> LS = new LinkedList<String>();
      LS.add("NP");
      int NPnode_ID = breadthFirst_Search(parseTree.children(), LS);
      if (NPnode_ID !=-1)
      {
	      Tree temp = parseTree.children()[NPnode_ID];
	      //int subjectNode_ID = breadthFirst_Search(NP.children(), Nouns);
	//      if (subjectNode_ID != -1)
	//        Subject =NP.getChild(subjectNode_ID).firstChild().label().value();
	
	      List<String> nouns2 = new LinkedList<String>();
	      nouns2.add("NP");
	      nouns2.addAll(Nouns);
	      boolean done = false;
	      while (!done)
	      {
	          NPnode_ID = breadthFirst_Search(temp.children(),nouns2);
	          if (NPnode_ID != -1)
	          {
	              if (temp.getChild(NPnode_ID).label().value().equals("NP"))
	              {
	                  temp = temp.getChild(NPnode_ID);
	              }
	              else
	              {
	                  Subject = temp.getChild(NPnode_ID).firstChild().label().value();
	                  done = true;
	              }
	          }
	          else
	          {
	        	  done = true;
	          }
	      }
      }
  }
  public void Extract_Predicate (Tree parseTree)
  {
      List<String> LS = new LinkedList<String>();
      LS.add("VP");
      int VPnode_ID = breadthFirst_Search(parseTree.children(), LS);
      if (VPnode_ID != -1)
      {
        Tree VP = parseTree.children()[VPnode_ID];
        breadthFirst_Search2(VP, Verbs, parseTree);
      }
      

  }
  public void Extract_Object (Tree parseTree)
  {
      Objects = new LinkedList<String>();
      List<Tree> Siblings = PredicateTree.siblings(parseTree);
      int i = 0;
      boolean doneAll = false;
      while (i < Siblings.size() && !doneAll)
      {
          if (Siblings.get(i).label().value().equals("NP") || Siblings.get(i).label().value().equals("PP"))
          {
              Tree temp = Siblings.get(i);
              List<String> nouns2 = new LinkedList<String>();
              nouns2.add("NP");
              nouns2.addAll(Nouns);
              int object_ID = 0;
              boolean done = false;
              while (!done)
              {
                  object_ID = breadthFirst_Search(temp.children(),nouns2);
                  if (object_ID != -1)
                  {
                      if (temp.getChild(object_ID).label().value().equals("NP"))
                      {
                          temp = temp.getChild(object_ID);
                      }
                      else
                      {
                          Objects.add(temp.getChild(object_ID).firstChild().label().value());
                          doneAll = true;
                          done = true;
                      }
                  }
                  else
                      done = true;
              }

          }
          else if (Siblings.get(i).label().value().equals("ADJP"))
          {
              int object_ID = breadthFirst_Search(Siblings.get(i).children(), Adjectives);
              if (object_ID != -1)
                Objects.add(Siblings.get(i).getChild(object_ID).firstChild().label().value());
          }
          i++;
      }

  }

  public boolean  Extract_Triples(Tree parseTree)   //S Tree
  {
      Extract_Subject(parseTree);
      if (Subject != "")
      {
        Extract_Predicate(parseTree);
        if (Predicate != "")
        {
            Extract_Object(parseTree);
            return true;
        }
      }
      return false;
  }
}
