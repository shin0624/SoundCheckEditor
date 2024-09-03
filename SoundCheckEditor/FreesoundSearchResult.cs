using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SoundCheckEditor;

public class FreesoundSearchResult//Freesound API의 검색 결과를 표현할 클래스
{
    public List<FreesoundSound> results { get; set; }//검색 결과 리스트
}