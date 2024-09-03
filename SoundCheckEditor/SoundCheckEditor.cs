using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SoundCheckEditor : EditorWindow
{
    // SoundCheckEditor : FreeSound의 API Key를 사용하여, 유니티 에디터 내에서 Freesound에 존재하는 License Free의 사운드를 검색하고 play해볼 수 있는 기능
    // 유의사항 : Json 데이터 처리를 위해 Newtonsoft json 3.2.1버전을 임포트해야 하는데, 오류가 발생하는 경우가 존재. 이럴 때는 asset폴더 내 plugin 폴더에 직접 Newtonsoft.json dll파일을 넣어주면 해결

    private string searchQuery = ""; // 검색어 입력 필드
    private List<SoundResult> soundResults = new List<SoundResult>(); // 검색 결과 리스트
    private static readonly HttpClient client = new HttpClient(); // HttpClient 싱글턴 사용. HttpClient는 인스턴스화하는 비용이 비교적 크기 때문에, 가능한 한 재사용하는 것이 좋음
    private Vector2 scrollPosition;//스크롤 위치 


    [MenuItem("Tools/SoundCheckEditor")]
    public static void ShowWindow()
    {
        GetWindow<SoundCheckEditor>("Sound Check Editor");
    }

    private void OnGUI()
    {
        // GUI 그리기
        GUILayout.Label("Search for Sound Effect You Want(원하는 사운드 이펙트를 검색하세요)\n", EditorStyles.boldLabel);
        GUILayout.Label("This system uses the API from https://freesound.org/.(이 시스템은 https://freesound.org/의 API를 사용합니다.)\n", EditorStyles.whiteLabel);
        searchQuery = EditorGUILayout.TextField("Search Sound Effect", searchQuery);// 검색어 입력

        if (GUILayout.Button("Search"))//Search버튼 클릭 시 서치사운드 호출
        {
            SearchSound(searchQuery);
        }

        if (soundResults != null && soundResults.Count >0)// 검색 결과가 있을 때만 스크롤 뷰와 결과 목록을 표시한다.
        {
            //스크롤 가능한 영역을 생성. 현재 에디터 창의 너비와 높이에 맞게.
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 50));//50:하단 버튼 높이);

            foreach (var result in soundResults)//검색결과 리스트를 foreach로 순회. 각 항목을 표시
            {
                GUILayout.Label(result.name);// 사운드 이름 표시

                if (GUILayout.Button("Play"))//play버튼 클릭 시 사운드 페이지로 이동
                {
                    // Freesound의 오디오 페이지 URL 포맷: https://freesound.org/people/{username}/sounds/{id}/
                    string url = $"https://freesound.org/people/{result.username}/sounds/{result.id}/";
                    Application.OpenURL(url);//url을 웹 브라우저에서 열 수 있음.
                }
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No Results Found.");
        }
    }

    private async void SearchSound(string query)
        // Freesound의 api키와 검색 쿼리를 사용하여 api요청을 보낼 URL을 생성한다.
    {
        string apiKey = ""; // Freesound의 API 키
        string url = $"https://freesound.org/apiv2/search/text/?query={query}&token={apiKey}";

        try
        {
            //비동기적으로 HTTP GET 요청을 보낸 후 응답을 받는다. 생성한 URL을 사용.
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();// 요청이 성공했는지 확인
            string jsonResponse = await response.Content.ReadAsStringAsync();//응답의 JSON 문자열을 읽는다.

            soundResults = ParseSoundResults(jsonResponse);//JSON 문자열을 파싱하여 검색 결과 리스트를 업데이트
            Debug.Log("Sound Data fetched successfully");
        }
        catch (HttpRequestException e)//요청 실패 시
        {
            Debug.LogError($"Error fetching sound data: {e.Message}");
        }
    }

    private List<SoundResult> ParseSoundResults(string json)
    {
       // Debug.Log($"Received JSON: {json}");

        var searchResults = JsonConvert.DeserializeObject<FreesoundSearchResult>(json);//JSON 문자열을 FreesoundSearchResult 객체로 변환.

        if (searchResults == null || searchResults.results == null)
        {
            // 검색 결과가 없거나 json 구조가 유효하지 않을 경우 출력되는 에러
            Debug.LogError("No search results found or invalid JSON structure.");
            return new List<SoundResult>();//빈 리스트를 반환토록 한다.
        }

        List<SoundResult> results = new List<SoundResult>();

        foreach (var sound in searchResults.results)// 검색 결과 리스트를 순회하여, SoundResult 객체로 변환 후 리스트에 추가
        {
            results.Add(new SoundResult
            {
                id = sound.id,
                name = sound.name,
                license = sound.license,
                username = sound.username
            });
        }

        return results;//변환된 검색 결과 리스트 반환
    }

  




}
