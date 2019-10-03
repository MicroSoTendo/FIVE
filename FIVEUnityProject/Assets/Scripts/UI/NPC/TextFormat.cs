using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFormat : MonoBehaviour
{
    public string[] Description;
    public void GenerateDescription()
    {
        Description = new string[12];
        Description[0] = "What should I do? I have to keep myself from shutting down by using <b><color=red>BATTERIES</color></b> or <b><color=green>CHARGING STATION</color></b> nearby..But I do not have enough energy to do that. ";
        Description[1] = "I want to have my solar system set up. But I have heard that the only place that I can found the <b>SiO2</b> is <b><color=blue>TATA Desert</color></b>.";
        Description[2] = "Human once said <b>'Higher the return higher will be the risk'</b>. What does that mean?";
        Description[3] = "Did you see the <b>BRACKETS</b> while you are looking around? Those are the <b><color=red>MATERIALs</color></b> that you can get charge of or build new items in the <b><color=green>BlACKSMITH SHOP​</color></b>. It is right across the  <b><color=green>RJSHOP</color></b>.";
        Description[4] = "I have heard that there is a <b>Portal</b> ​somewhere in the <b><color=blue>Rusty City</color></b>. Can I use that too? I want to go to the forest and find out what a <b><color=red>TREE</color></b> looks like.";
        Description[5] = "My programmer <b>Gen</b> used to struggled with networking.. It is not even a problem to me.";
        Description[6] = "Are you calling me <b>Wally</b>? Good name! I prefer you calling me WALL-E then.Wait..Am I spelling the name wrong ? ";
        Description[7] = "Have you ever heard <b>Wen</b>? She is the greatest human architect I have ever seen! Oh but human was extincted last year.. Sad story.";
        Description[8] = "I can't hear what <b>Xinyu</b> said at all. Maybe I should get my Tuner fixed. Wait.. How did I hear you then?";
        Description[9] = "Have you ever saw an Orange walking around?  Nope it is definitely not a human. It is a walking Orange. ";
        Description[10] = "Welcome to the league of Cyber.";
        Description[11] = "Do you have a good machine learning strategy?";
    }
}
