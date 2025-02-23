using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BackgroundControll : MonoBehaviour
{
    public Transform carrr;
    public Transform[] plochki;
    Vector3 sega = new Vector3(0, 0, 0);

    private void Start() {
        sega = plochki[0].position;
    }

    private void FixedUpdate() {
        float dist = 10000000000;
        int koi = 0;
        for (int q = 0; q < plochki.Length; q++) {
            if (Vector3.Distance(carrr.position, plochki[q].position) <dist ) {
                dist=Vector3.Distance(carrr.position,plochki[q].position);
                koi = q;
            }
        }
        if (koi!=0) {
            Vector3 ofss = plochki[koi].position - plochki[0].position;
            for (int q = 0; q < plochki.Length; q++) {
                plochki[q].position += ofss;
            }
        }

    }
}
