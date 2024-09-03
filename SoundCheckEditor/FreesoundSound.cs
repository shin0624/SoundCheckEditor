using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreesoundSound//Freesound API의 사운드 항목을 표현할 클래스
{
    public int id { get; set; }// 사운드 ID
    public string name { get; set; }//사운드 이름
    public string license { get; set; }//라이센스 정보
    public string username { get; set; }//사운드를 업로드한 사용자 이름
}