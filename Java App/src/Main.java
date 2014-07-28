import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import javax.xml.parsers.ParserConfigurationException;
import org.xml.sax.SAXException;
import srt_processing.SRTException;
import Entities.Scene;
import Entities.Triple;
import Helpers.GeneralHelper;
import Helpers.OfflineTriplesExtractor;
import Helpers.OfflineTriplesExtractor2;
import Helpers.OnlineTriplesExtractor;
import Helpers.StanfordCoreNLPHelper;
import edu.stanford.nlp.pipeline.Annotation;

public class Main 
{
	public static void main(String[] args) 
	{
		// hide errors & loading messages
		//System.err.close(); 
		
	    String output = "";
	    int shortTextLimit = 3;
		String srtFile = "7.srt";
		String inputText = "that is dog." +
				"";
		int triplesExtractionMethod = 4;
		Boolean withCoreference = false;
		Boolean withLemmatization = true;
		Boolean theInputIsText = true;
		
		// get the values from console
		if (args.length >= 4)
	    {
	    	triplesExtractionMethod = Integer.parseInt(args[0]);
	    	withCoreference = Boolean.parseBoolean(args[1]);
	    	withLemmatization = Boolean.parseBoolean(args[2]);
	    	srtFile = args[3];
	    	inputText = args[3];
	    	theInputIsText = false;
	    }
		if (args.length > 4)
	    {
	    	theInputIsText = Boolean.parseBoolean(args[4]);
	    }
		
		if (theInputIsText)
		{
			// remove  strange characters from text
			if (inputText.length() > shortTextLimit) // if the text is not too short
			{
				inputText = GeneralHelper.cleanString(inputText);
			}
			
			
			// replace the original text with the coreferenced text
			if (withCoreference && inputText.length() > shortTextLimit) // if the text is not too short
			{
				Annotation annotation = StanfordCoreNLPHelper.getAnnotation(inputText);
				inputText = StanfordCoreNLPHelper.getCoreferencedText(annotation);
			}
		    
		    
		    //extract the triples from each scene
		    if (inputText.length() > shortTextLimit) // if the text is not too short
		    {
			    try 
				{
			    	List<Triple> triples = null;			    	
			    	switch (triplesExtractionMethod) 
			    	{
			            case 1: 
			            {
			            	triples = OfflineTriplesExtractor.getTriples("", inputText);
			            	break;
			            }
			            case 2: 
			            {
			            	triples = OfflineTriplesExtractor2.getTriples("", inputText);
			            	break;
			            }
			            case 3: 
			            {
			            	triples = OnlineTriplesExtractor.getTriples("", inputText);
			            	break;
			            }
			            case 4: 
			            {
			            	List<Triple> triples1 = OfflineTriplesExtractor.getTriples("", inputText);
			            	List<Triple> triples2 = OfflineTriplesExtractor2.getTriples("", inputText);
			            	triples = GeneralHelper.union(triples1, triples2);
			            	break;
			            }
			            case 5: 
			            {
			            	List<Triple> triples1 = OfflineTriplesExtractor.getTriples("", inputText);
			            	List<Triple> triples2 = OfflineTriplesExtractor2.getTriples("", inputText);
			            	List<Triple> triples3 = OnlineTriplesExtractor.getTriples("", inputText);
			            	triples = GeneralHelper.union(triples1, triples2);
			            	triples = GeneralHelper.union(triples, triples3);
			            	break;
			            }
			    	}
					for (Triple tr : triples)
					{
						if (tr.isValid())
						{
							if (withLemmatization)
							{
								output += tr.getLemmatizedString() + "\n";
							}
							else	
							{
								output += tr + "\n";
							}
						}
					}
				} 
				catch (IOException e) 
				{
					output += "error n0000021" + "\n";
					e.printStackTrace();
				} 
				catch (ParserConfigurationException e) 
				{
					output += "error n0000022" + "\n";
					e.printStackTrace();
				} 
				catch (SAXException e) 
				{
					output += "error n0000023" + "\n";
					e.printStackTrace();
				}  	
		    }  
		}
		else
		{
			//extract the scenes from SRT file
			List<Scene> scenesList = new ArrayList<>();
			try
			{
				scenesList = GeneralHelper.processSRT(srtFile);
			}
			catch (SRTException e) 
			{
				output += "error n0000011" + "\n";
				e.printStackTrace();
			}
			
			//iterate through all scenes to process
			int i = 0;
			for (Scene scene : scenesList)
			{
				
				String text = scene.getText();
				
				// remove  strange characters from text
				if (text.length() > shortTextLimit) // if the text is not too short
				{
					text = GeneralHelper.cleanString(text);
				}
				
				
				// replace the original text with the coreferenced text
				if (withCoreference && text.length() > shortTextLimit) // if the text is not too short
				{
					Annotation annotation = StanfordCoreNLPHelper.getAnnotation(text);
				    text = StanfordCoreNLPHelper.getCoreferencedText(annotation);	
				}
			    
			    // for debugging purposes
			    System.err.println("-------------------" + "processing scene: " + ++i + "-------------------");
			    System.err.println(scene.getText());
			    System.err.println("-------------------");
			    System.err.println(text);
			    System.err.println("-------------------");
			    // end
			    
			    //extract the triples from each scene
			    if (text.length() > shortTextLimit) // if the text is not too short
			    {
				    try 
					{
				    	List<Triple> triples = null;			    	
				    	switch (triplesExtractionMethod) 
				    	{
				            case 1: 
				            {
				            	triples = OfflineTriplesExtractor.getTriples(scene.getTime(), text);
				            	break;
				            }
				            case 2: 
				            {
				            	triples = OfflineTriplesExtractor2.getTriples(scene.getTime(), text);
				            	break;
				            }
				            case 3: 
				            {
				            	triples = OnlineTriplesExtractor.getTriples(scene.getTime(), text);
				            	break;
				            }
				            case 4: 
				            {
				            	List<Triple> triples1 = OfflineTriplesExtractor.getTriples(scene.getTime(), text);
				            	List<Triple> triples2 = OfflineTriplesExtractor2.getTriples(scene.getTime(), text);
				            	triples = GeneralHelper.union(triples1, triples2);
				            	break;
				            }
				            case 5: 
				            {
				            	List<Triple> triples1 = OfflineTriplesExtractor.getTriples(scene.getTime(), text);
				            	List<Triple> triples2 = OfflineTriplesExtractor2.getTriples(scene.getTime(), text);
				            	List<Triple> triples3 = OnlineTriplesExtractor.getTriples(scene.getTime(), text);
				            	triples = GeneralHelper.union(triples1, triples2);
				            	triples = GeneralHelper.union(triples, triples3);
				            	break;
				            }
				    	}
						for (Triple tr : triples)
						{
							if (tr.isValid())
							{
								if (withLemmatization)
								{
									output += tr.getLemmatizedString() + "\n";
									// for debugging purposes
									System.err.println(tr);
									// end
								}
								else
								{
									output += tr + "\n";
									// for debugging purposes
									System.err.println(tr);
									// end
								}
							}
						}
					} 
					catch (IOException e) 
					{
						output += "error n0000021" + "\n";
						e.printStackTrace();
					} 
					catch (ParserConfigurationException e) 
					{
						output += "error n0000022" + "\n";
						e.printStackTrace();
					} 
					catch (SAXException e) 
					{
						output += "error n0000023" + "\n";
						e.printStackTrace();
					}  	
			    }  
			}	
		}
		
	    System.out.println(output);	    
	}
}
