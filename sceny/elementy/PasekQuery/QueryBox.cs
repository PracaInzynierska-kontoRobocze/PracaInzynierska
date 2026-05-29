using Godot;
using System;

public partial class QueryBox : Control
{
    public void SetVisible(int id)
    {
        foreach (Control child in GetChildren())
        {
            if ((int)child.GetMeta("TypQuery_Id")==id)
            {
                child.Visible = true;
            }
            else 
            {
                child.Visible = false;

            }
        }



    }

}
