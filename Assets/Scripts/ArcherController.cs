using UnityEngine;

public class ArcherController : MonoBehaviour
{
    public GameObject Archer;
    private Animator Animator_;

    void Start()
    {
        Animator_ = Archer.GetComponent<Animator>();
    }

    void Update()
    {
        var Dir = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            Dir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Dir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Dir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Dir += Vector3.back;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dir = Vector3.zero;
            Animator_.SetTrigger("Attack1Trigger");
        }

        if (Animator_.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Attack1")
        {
            Dir = Vector3.zero;
        }

        if (!Mathf.Approximately(Dir.sqrMagnitude, 0))
        {
            Animator_.SetBool("Moving", true);
            Archer.transform.Translate(Dir.normalized * Time.deltaTime * 10, Space.World);
            Archer.transform.LookAt(Archer.transform.position + Dir.normalized * 10);
        }
        else
        {
            Animator_.SetBool("Moving", false);
        }

        Camera.main.transform.position = Archer.transform.position + new Vector3(0, 15, -15);
        Camera.main.transform.rotation = Quaternion.AngleAxis(45, new Vector3(1, 0, 0));
    }

    public void Hit()
    {
        Debug.Log("Hit");
    }

    public void FootR()
    {
    }

    public void FootL()
    {
    }
}