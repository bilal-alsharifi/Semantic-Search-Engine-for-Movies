package Entities;

public class Sentence 
{
	private String Action = "", subject = "";
	public String getAction() 
	{
		return Action;
	}
	
	public String getSubject() 
	{
		return subject;
	}
	
	public Sentence(String sub, String act) 
	{
		this.Action = act;
		this.subject = sub;
	}
}