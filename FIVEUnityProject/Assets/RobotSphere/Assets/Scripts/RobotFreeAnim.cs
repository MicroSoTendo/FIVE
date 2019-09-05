using UnityEngine;

[ExecuteInEditMode]
public class RobotFreeAnim
{
    private GameObject gameObject;

    Animator anim;

    public RobotFreeAnim(GameObject gameObject)
    {
        this.gameObject = gameObject;
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update(RobotSphere.RobotState currState)
    {
        UpdateAnim(currState);
    }

    void UpdateAnim(RobotSphere.RobotState currState)
    {
        if (currState == RobotSphere.RobotState.Idle)
        {
            anim.SetBool("Walk_Anim", false);
        }
        else if (currState == RobotSphere.RobotState.Walk)
        {
            anim.SetBool("Walk_Anim", true);
        }
    }

    void CheckKey()
    {
        // Walk
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("Walk_Anim", true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetBool("Walk_Anim", false);
        }

        // Rotate Left
        if (Input.GetKey(KeyCode.A))
        {
        }

        // Rotate Right
        if (Input.GetKey(KeyCode.D))
        {
        }

        // Roll
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (anim.GetBool("Roll_Anim"))
            {
                anim.SetBool("Roll_Anim", false);
            }
            else
            {
                anim.SetBool("Roll_Anim", true);
            }
        }

        // Close
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!anim.GetBool("Open_Anim"))
            {
                anim.SetBool("Open_Anim", true);
            }
            else
            {
                anim.SetBool("Open_Anim", false);
            }
        }
    }

}
