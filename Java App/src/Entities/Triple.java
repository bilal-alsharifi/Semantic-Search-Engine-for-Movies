package Entities;
import Helpers.GeneralHelper;
import Helpers.StanfordCoreNLPHelper;

public class Triple 
{
	private String sceneTime;
	private String su;
	private String pr;
	private String ob;
	
	public Triple(String sceneTime, String su, String pr, String ob, Boolean withCleaning) 
	{
		this.sceneTime = sceneTime;
		if (withCleaning)
		{
			this.su = GeneralHelper.cleanSuPrOb(su.replace(" ", "_"));
			this.pr = GeneralHelper.cleanSuPrOb(StanfordCoreNLPHelper.cleanPredicate(pr).replace(" ", "_"));
			this.ob = GeneralHelper.cleanString(ob.replace(" ", "_").replace("'", "").replace(".", ""));
		}
		else
		{
			this.su = GeneralHelper.cleanSuPrOb(su);
			this.pr = GeneralHelper.cleanSuPrOb(pr);
			this.ob = GeneralHelper.cleanSuPrOb(ob);
		}
	}

	public String getSceneTime() 
	{
		return this.sceneTime;
	}
	
	public String getSubject() 
	{
		return this.su;
	}

	public String getPredicate() 
	{
		return this.pr;
	}

	public String getObject() 
	{
		return this.ob;
	}
	
	public Boolean isValid()
	{
		if (this.su.length() > 0 && this.pr.length() > 0 && this.ob.length() > 0  && !this.pr.matches("\\d.*"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public String getLemmatizedString() 
	{
		String sep = ";";
		return this.sceneTime +  sep + StanfordCoreNLPHelper.getLemma(this.su) + sep + StanfordCoreNLPHelper.getLemma(this.pr) + sep + StanfordCoreNLPHelper.getLemma(this.ob);
	}
	
	@Override
	public String toString() 
	{
		String sep = ";";
		return this.sceneTime +  sep + this.su + sep + this.pr + sep + this.ob;
	}
	
	@Override
	public boolean equals(Object obj)
	{
		boolean result = true;
		Triple tr = (Triple)obj;
		if (!this.toString().equals(tr.toString()))
		{
			result = false;
		}
		return result;
	}
	
	@Override
	public int hashCode()
	{
		return (this.toString()).hashCode();
	}
}