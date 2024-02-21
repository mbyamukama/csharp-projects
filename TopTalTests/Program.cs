namespace TopTalTests
{
  internal class Program
  {

    public static string findword(string[] rules)
    {
      //get unique letters
      List<string> unique = new List<string>();
      foreach (string rule in rules)
      {
        string l1 = rule[0].ToString();
        string l2 = rule[2].ToString();
        if (!unique.Contains(l1)) unique.Add(l1);
        if (!unique.Contains(l2)) unique.Add(l2);
      }
      List<string> word = new List<string>(unique); //ICBLEPUR
      word.Add("");

      //replace indices

      for (int i = 0; i < unique.Count; i++)
      {
        string letter= unique[i];
        foreach (string rule in rules)
        {
          if (rule.Contains(letter))
          {
            if (rule.IndexOf(letter) == 0) //letter is superior. get other, insert it after
            {
              int idx1 = word.IndexOf(letter);
              int idx2 = word.IndexOf(rule[2].ToString());
              word.RemoveAt(idx2);
              word.Insert(idx1 + 1, rule[2].ToString());
            }
            if (rule.IndexOf(letter) == 2) //letter is inferior. get 1st, insert this one after
            {
              int idx1 = word.IndexOf(rule[0].ToString());
              int idx2 = word.IndexOf(letter);
              word.RemoveAt(idx2);

              word.Insert(idx1 + 1, rule[2].ToString());
            }

          }
        }
      }
      
      return String.Join("", word);
    }
    static void Main(string[] args)
    {
      string[] rules = { "I>C", "B>L", "E>P", "P>U", "R>E", "U>B", "L>I", }; // CRILBUPE
      string word = findword(rules);
      Console.WriteLine(word);
    }
  }
}