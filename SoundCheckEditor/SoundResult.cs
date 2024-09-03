using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundResult// 검색 결과를 표현할 클래스
{
    public int id { get; set; }//사운드의 id
    public string name { get; set; }//이름
    public string license { get; set; }//라이센스 정보
    public string username { get; set; }//사운드를 업로드한 사용자 이름
}