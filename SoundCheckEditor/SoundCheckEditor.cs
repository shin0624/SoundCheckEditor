using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SoundCheckEditor : EditorWindow
{
    // SoundCheckEditor : FreeSound의 API Key를 사용하여, 유니티 에디터 내에서 Freesound에 존재하는 License Free의 사운드를 검색하고 play해볼 수 있는 기능

    private string searchQuery = ""; // 검색어 입력 필드
    private List<SoundResult> soundResults = new List<SoundResult>(); // 검색 결과 리스트
    private static readonly HttpClient client = new HttpClient(); // HttpClient 싱글턴 사용. HttpClient는 인스턴스화하는 비용이 비교적 크기 때문에, 가능한 한 재사용하는 것이 좋음
    private Vector2 scrollPosition;//스크롤 위치 

    private string nextPageUrl = null;//다음 페이지 url 저장
    private string prevPageUrl = null;//이전 페이지 url 저장

    private string apiKey;//apikey는 유저가 직접 입력할 수 있다.
    private bool needSearch = false;//검색이 필요한지 여부를 추적

    [MenuItem("Tools/SoundCheckEditor")]
    public static void ShowWindow()
    {
        GetWindow<SoundCheckEditor>("Sound Check Editor");
    }

    private void OnEnable()// 에디터창이 활성화되면 EditorPrefs에서 저장된 apikey를 불러온다.
    {
        apiKey = EditorPrefs.GetString("FreesoundAPIKey", "");//기본값은 빈 문자열
        EditorApplication.update += Update;//업데이트 메서드를 등록
    }

    private void OnDisable()
    {
        EditorApplication.update-=Update;//업데이트 메서드 해제
    }

    private void Update()
    {
        if(needSearch)// 검색 요청 처리
        {
            needSearch = false;//검색 후 상태 리셋
            SearchSound(searchQuery);
        }
    }

    private void OnGUI()
    {
        // GUI 그리기
        GUILayout.Label("Search for Sound Effect You Want(원하는 사운드 이펙트를 검색하세요)\n", EditorStyles.boldLabel);
        GUILayout.Label("This system uses the API from https://freesound.org/.(이 시스템은 https://freesound.org/의 API를 사용합니다.)\n", EditorStyles.whiteLabel);

        DrawApiKeyField();//api키 입력 및 저장 ui
        
        searchQuery = EditorGUILayout.TextField("Search Sound Effect", searchQuery);// 검색어 입력

        if (GUILayout.Button("Search") && !string.IsNullOrEmpty(apiKey))//Search버튼 클릭 시 + apikey 입력 시 서치사운드 호출
        {
            //SearchSound(searchQuery);
            needSearch=true;//검색이 필요함을 표시
        }

        if (soundResults != null && soundResults.Count > 0)// 검색 결과가 있을 때만 스크롤 뷰와 결과 목록을 표시한다.
        {
            DrawSoundResults();
            DrawPaginationButtons();
        }
        else if(!needSearch)//검색을 하지 않았을 때
        {
            GUILayout.Label("No Results Found.");
        }
    }

    private async void SearchSound(string query)// Freesound의 api키와 검색 쿼리를 사용하여 api요청을 보내 사운드를 검색. 입력한 데이터에 따라 동적으로 URL 생성,호출
    {
        if (string.IsNullOrEmpty(apiKey))//오류처리
        {
            EditorUtility.DisplayDialog("API Key Missing", "Please enter and save your API Key first.", "OK");
            return;
        }
        string url = $"https://freesound.org/apiv2/search/text/?query={query}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);// HttpRequestMessage 객체 생성 및 인증 헤더 설정
        request.Headers.Add("Authorization", $"Token {apiKey}");
        await FetchSoundData(request); // 주어진 url로 freesound 데이터를 검색하는 메서드. 페이지 이동 시 사용
    }

    private async Task FetchSoundData(HttpRequestMessage request) //주어진 URL로 freesound 데이터를 비동기적으로 가져오는 메서드.
    {
        try
        {
            HttpResponseMessage response = await client.SendAsync(request);//비동기적으로 HTTP GET 요청을 보낸 후 응답을 받는다. 생성한 URL을 사용.
            if (response.IsSuccessStatusCode) // 요청이 성공했는지 확인
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();//응답의 JSON 문자열을 읽는다.
                soundResults = ParseSoundResults(jsonResponse);//JSON 문자열을 파싱하여 검색 결과 리스트를 업데이트
            }
            else
            {
                HandleApiError(response);
            }
        }
        catch (HttpRequestException e)//요청 실패 시
        {
            Debug.LogError($"Error fetching sound data: {e.Message}");
            EditorUtility.DisplayDialog("API Request Error", $"Error fetching sound data: {e.Message}", "OK");
        }
    }

    private async void SearchSoundWithUrl(string url)// 주어진 URL을 사용하여 Freesound API를 호출. 페이지 넘기기 기능을 위해 사용.
    {   //SearchSound메서드와의 차이 : SearchSoundWithUrl은 외부에서 제공되는 URL 사용. SearchSound는 직접 URL생성.
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Token {apiKey}");
        await FetchSoundData(request);
    }

    private List<SoundResult> ParseSoundResults(string json)//freesound API 응답을 파싱하여 검색 결과를 반환하는 메서드
    {
        //Debug.Log($"Received JSON: {json}");
        var searchResults = JsonConvert.DeserializeObject<FreesoundSearchResult>(json);//JSON 문자열을 FreesoundSearchResult 객체로 변환.

        if (searchResults == null || searchResults.results == null)// 검색 결과가 없거나 json 구조가 유효하지 않을 경우 출력되는 에러
        {
            Debug.LogError("No search results found or invalid JSON structure.");
            return new List<SoundResult>();//빈 리스트를 반환토록 한다.
        }
        //다음페이지와 이전 페이지의 URL 설정
        nextPageUrl = CleanUrl(searchResults.next);
        prevPageUrl = CleanUrl(searchResults.previous);

       // Debug.Log($"Next Page URL: {nextPageUrl}");
       // Debug.Log($"Previous Page URL: {prevPageUrl}");

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

    private void DrawSoundResults()// 검색결과를 보여주는 메서드
    {
        int maxResultToShow = 10;//UI 한 페이지에 보여지는 최대 사운드 개수
        int resultToDisplay = Mathf.Min(soundResults.Count, maxResultToShow);

        //스크롤 가능한 영역을 생성. 높이를 고정하고 빈 공간을 추가하여 next,prev버튼이 가려지지 않게 조치
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(300));
        for (int i = 0; i < resultToDisplay; i++)
        {
            GUILayout.Label(soundResults[i].name);// 사운드 이름을 8개까지만 표시
            if (GUILayout.Button("Play"))//play버튼 클릭 시 사운드 페이지로 이동
            {
                // Freesound의 오디오 페이지 URL 포맷: https://freesound.org/people/{username}/sounds/{id}/
                string url = $"https://freesound.org/people/{soundResults[i].username}/sounds/{soundResults[i].id}/";
                Application.OpenURL(url);//url을 웹 브라우저에서 열 수 있음.
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawPaginationButtons()//페이지 넘기기 버튼 출력 메서드
    {
            //빈 공간 추가 : 검색 결과와 페이지 넘기기 버튼사이에 여유 공간 확보
            GUILayout.Space(20);//버튼과 검색 결과 사이에 20픽셀의 여백 추가

            //페이지 넘기기 버튼
            EditorGUILayout.BeginHorizontal();
            {
                // 이전 페이지 버튼: prevPageUrl이 null이면 비활성화
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(prevPageUrl));
                if (GUILayout.Button("< Previous Page", GUILayout.Height(30))) // 버튼 크기 수정
                {
                    SearchSoundWithUrl(prevPageUrl);
                }
                EditorGUI.EndDisabledGroup();

                // 다음 페이지 버튼: nextPageUrl이 null이면 비활성화
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(nextPageUrl));
                if (GUILayout.Button("Next Page >", GUILayout.Height(30))) // 버튼 크기 수정
                {
                    SearchSoundWithUrl(nextPageUrl);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
    }

    private string CleanUrl(string url)// next, prev 페이지 url 포맷 중 필요없는 부분을 제거
    {
        if (string.IsNullOrEmpty(url)) return url;
        return url.Replace("&weights=", "");
    }

    private void DrawApiKeyField() //사용자가 에디터 창에서 직접 본인의 API키를 입력하고 저장할 수 있도록 설정
    {   
        GUILayout.Label("Freesound API Key", EditorStyles.boldLabel);
        string newApiKey = EditorGUILayout.TextField("API Key", apiKey);
        if (newApiKey != apiKey)
        {
            apiKey = newApiKey;
        }

        if (GUILayout.Button("Save API Key"))//세이브 버튼 클릭 시 apikey를 EditorPrefs에 저장
        {
            EditorPrefs.SetString("FreesoundAPIKey", apiKey);
            EditorUtility.DisplayDialog("API Key Saved", "Your API Key has been saved successfully.", "OK");
        }
    }

    private void HandleApiError(HttpResponseMessage response)//api키 에러 핸들링을 위한 메서드
    {
        string errorMessage = $"API Error : {response.ReasonPhrase}";
        if(response.StatusCode==System.Net.HttpStatusCode.Unauthorized)//인증오류 시
        {
            errorMessage = "Unauthorized: Your API Key might be invalid or expired.";
        }
        else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)//잘못된 접근 시
        {
            errorMessage = "Forbidden: You do not have permission to access this resource.";
        }
        Debug.LogError(errorMessage);
        EditorUtility.DisplayDialog("API Error", errorMessage, "OK");
    }
 }

