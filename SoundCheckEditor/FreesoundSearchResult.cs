using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SoundCheckEditor;

public class FreesoundSearchResult//Freesound API의 검색 결과를 표현할 클래스
{    
    public string next { get; set; }//다음 페이지 url
    public string previous { get; set; }//이전 페이지 url
    public List<FreesoundSound> results { get; set; }//검색 결과 리스트

}