package Helpers;
import java.io.File;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import srt_processing.SRT;
import srt_processing.SRTException;
import srt_processing.SRTInfo;
import srt_processing.SRTReader;
import srt_processing.SRTTimeFormat;
import Entities.*;

public class GeneralHelper 
{
	public static List<Scene> processSRT(String srtFile) throws SRTException
	{
		List<Scene> result = new ArrayList<Scene>();
		File file = new File(srtFile);
		SRTInfo info = SRTReader.read(file);
        String sceneTime = "";
        for (SRT s : info)
        {
            String sceneText = "";
            for (String line: s.text)
            {
            	sceneText += line + " ";
            }    
            sceneTime = SRTTimeFormat.format(s.startTime) + " -> " + SRTTimeFormat.format(s.endTime);
            Scene scene = new Scene(sceneTime, sceneText);
            result.add(scene);
        }
        return result;
	}
	
	public static String cleanString(String s) 
	{
	    String result = s.replaceAll("<i>|</i>|[^\\w ! \" '  , . : ?]", "");
	    return result;
	}
	
	public static String cleanSuPrOb(String s) 
	{
	    String result = s.replaceAll("[! \" '  , . : ?]", "");
	    return result;
	}
	
	public static <T> List<T> union(List<T> list1, List<T> list2) 
	{
        Set<T> set = new HashSet<T>();
        set.addAll(list1);
        set.addAll(list2);
        return new ArrayList<T>(set);
    }
}
