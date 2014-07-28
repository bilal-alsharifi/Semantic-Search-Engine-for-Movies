package Entities;

public class Scene 
{
	private String time;
	private String text;
	public Scene(String time, String text)
	{
		this.time = time;
		this.text = text;
	}
	public String getTime()
	{
		return this.time;
	}
	public String getText()
	{
		return this.text;
	}
	public String toString()
	{
		return "[" + this.time + "," + this.text + "]";
	}
}
