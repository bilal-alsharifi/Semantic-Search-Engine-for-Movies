package Helpers;
import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import Entities.*;
public class OnlineTriplesExtractor 
{

	private static String getResponse(String text) throws IOException
	{
		String urlParameters = text;
		URL url = new URL("http://enrycher.ijs.si/run");
		HttpURLConnection connection = (HttpURLConnection) url.openConnection();           
		connection.setDoOutput(true);
		connection.setDoInput(true);
		connection.setInstanceFollowRedirects(false); 
		connection.setRequestMethod("POST"); 
		connection.setRequestProperty("Content-Type", "application/x-www-form-urlencoded"); 
		connection.setRequestProperty("charset", "utf-8");
		connection.setRequestProperty("Content-Length", "" + Integer.toString(urlParameters.getBytes().length));
		connection.setUseCaches (false);
		DataOutputStream writer = new DataOutputStream(connection.getOutputStream ());
		writer.writeBytes(urlParameters);
		writer.flush();		
		String line;
		StringBuffer response = new StringBuffer();
		BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
		while ((line = reader.readLine()) != null) 
		{
		    response.append(line + "\r");
		}		
		writer.close();
		reader.close();
		connection.disconnect();
		return response.toString();
	}
	
	public static List<Triple> getTriples(String sceneTime, String text) throws ParserConfigurationException, SAXException, IOException
	{
		List<Triple> result = new ArrayList<>();
        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        DocumentBuilder db = dbf.newDocumentBuilder();
        String response = getResponse(text);
        InputSource is = new InputSource(new StringReader(response));
        Document doc = db.parse(is);
        doc.getDocumentElement().normalize();
        NodeList tripleNodes = doc.getElementsByTagName("assertion");
        for (int i = 0; i < tripleNodes.getLength(); i++)
        {
        	Node tripleNode = tripleNodes.item(i);
        	Element tripleElement = (Element) tripleNode;
        	Element suElement = (Element)(tripleElement.getElementsByTagName("subject").item(0));
        	String su = suElement.getAttribute("displayName");
        	Element prElement = (Element)(tripleElement.getElementsByTagName("verb").item(0));
        	String pr = prElement.getAttribute("displayName");
        	Element obElement = (Element)(tripleElement.getElementsByTagName("object").item(0));
        	String ob = obElement.getAttribute("displayName");
        	Triple triple = new Triple(sceneTime, su, pr, ob, true);
        	result.add(triple);
        }
        return result;    
	}
}
