# SoundCheckEditor
- Unity Engine 내에서 필요한 사운드를 즉시 Freesound에서 검색하여 플레이할 수 있는 커스텀에디터
- Freesound : 저작권 무료 음원사이트. (https://freesound.org/)
- API 문서는 http://www.freesound.org/docs/api/ 에서 찾을 수 있음.
- API 키는 https://www.freesound.org/apiv2/apply/ 에서 신청 가능 .

# 패키지 다운로드 링크
https://drive.google.com/file/d/1uJ7y2Jo4k8gwpsVRGpM7rKm6KBX5jjCj/view?usp=sharing

# 개발 환경 
- Unity Engine 2023.2.16f1
- Visual Studio Community 2022
- C#

# 기술 스택
- Unity Editor의 커스텀 에디터 기능
- EditorWindow
- EditorGUILayer / GUILayer
- EditorPrefs
- Freesound APIv2 : Freesound의 api키를 발급받아 사용
- Newtonesoft Json : json데이터 처리를 위해 사용
- System.Net.Http : HttpClient 사용을 위함
- System.Threading.Tasks : 비동기 작업 처리를 위함

# 개요
1. 게임에는 많은 사운드가 필요한데, 작업할 때 마다 필요한 사운드를 찾는 과정이 너무 번거로웠다. AssetStore처럼 음원 사이트도 에디터 내에서 접근할 수 있으면 편리성이 증대할 것이라 판단하여 사운드 체크 기능을 구현하였다.

# 사용 시 유의
- Json 데이터 처리를 위해 Newtonsoft json 3.2.1버전을 임포트(패키지 매니저 이름으로 찾기에서 com.unity.nuget.newtonsoft-json 입력 후 3.2.1버전 다운로드) 
- 오류가 발생하는 경우, asset폴더 내 plugin 폴더에 직접 Newtonsoft.json dll파일을 넣어주면 해결

# 기능
1. 원하는 사운드를 검색한다. 입력한 쿼리는 Freesound 사이트로 요청되고, 검색 결과가 에디터 창에 출력된다. 원하는 사운드의 play버튼을 클릭하면 Freesound의 해당 사운드 페이지로 연결된다.
2. 사용자 본인의 Freesound API키를 에디터 상에 입력하여 사용할 수 있다.

# 사용 예시
 ## Tool -> SoundCheckEditor 클릭
![toolbar](https://github.com/user-attachments/assets/a34742fa-0c4d-4cd1-9a42-17437d878ab5)

 ## 본인의 Freesound API KEY를 에디터 상에 입력(무료. 회원가입 시 발급 가능하며, 분당 60개 요청, 하루 2000개의 사용 제한 존재)
  Save API Key를 클릭하면 에디터 창 종료 후 재시작 시에도 값이 그대로 저장됨.
 ![API키 입력](https://github.com/user-attachments/assets/3aa4bbb1-2e2a-49cc-8c64-7e7cb1f38062)

 ## API 키 정상 입력 시
 ![API키정상입력시](https://github.com/user-attachments/assets/a6410d56-5e8f-4b82-8494-c565e768657e)

 ## Search Sound Effect의 텍스트필드에 검색하고싶은 사운드 명 입력 후 Search 클릭(딜레이 약간 존재)
 ![검색](https://github.com/user-attachments/assets/c1af1d22-77d6-4778-a6a1-edc37454fb07)

 ## 검색된 사운드 목록이 출력됨. 한 페이지 당 10개의 목록이 출력되며, 전-후 페이지 이동 가능
 ![검색결과](https://github.com/user-attachments/assets/a27f5091-17c9-4450-a2e2-3577b9c64eee)

 ## 목록 중 원하는 사운드 Play 클릭 시 Freesound의 해당 음향효과 검색 결과창으로 이동
 ![사이트연결2](https://github.com/user-attachments/assets/e027c82e-bb06-4384-a036-1ac89f44fdc1)

 ## API Key 오류 시(오류 조건 : 인증 오류 or 잘못된 접근)
 ![api키 에러시](https://github.com/user-attachments/assets/8f88e2ea-099e-4ca1-8351-3e988a6eec2c)
 ![에러 시 콘솔](https://github.com/user-attachments/assets/f55042a9-1939-42d7-97c1-dd15e9d134cd)
 
# 패키지 구성
![스크립트목록소개](https://github.com/user-attachments/assets/08f7cf70-f731-4c2c-85b8-e1e4c2a48779)
 1. FreesoundSearchResult : Freesound API의 검색 결과를 표현할 클래스
 2. FreesoundSound : Freesound API의 사운드 항목을 표현할 클래스
 3. SoundResult : 검색 결과를 표현할 클래스
 4. SoundCheckEditor : 에디터 구현 및 JSON 파싱

# 추가 예정 기능
1. 검색 목록 썸네일 기능
2. Unity 내에서 직접 오디오를 재생하는 기능
3. 에디터 윈도우에서 사이트 연결 없이 바로 미리듣기 기능

# 버전목록
- 2024.09.04 Ver.1 : https://drive.google.com/file/d/1F8qJMUQgJxh399z4ZgxFmvdOQGWK41ek/view?usp=drive_link
- 2024.09.05 Ver.2 : https://drive.google.com/file/d/1uJ7y2Jo4k8gwpsVRGpM7rKm6KBX5jjCj/view?usp=sharing

# 업데이트 노트
- Ver.1
    1. Freesound API를 사용해 에디터 상에서 음향효과 목록을 검색하고 접근할 수 있는 기능 설계

- Ver.2
    1. 매 프레임 호출되는 OnGUI의 비용 절감을 위해 EditorApplication.update를 이용해 상태기반 이벤트 처리 수행
    2. 페이지 넘기기 기능 추가 : Freesound의 next, previous응답을 사용하고, API 요청 시 Authorization 헤더를 포함하여 요청을 보냄. 
    3. 페이지 넘기기 UI 조절 : next page, previous page 버튼이 SoundResults에 가려 안보이게 됨을 방지하기 위해 빈 공간을 나누고 DisabledGroup으로 묶음. 
    4. next, prev URL의 불필요한 부분 제거 
    5. API Key 은닉 및 사용자친화적 입력 : 패키지 배포를 위해 APIKey를 숨기고, 사용자가 에디터 설정창을 통해 직접 본인의 APIKey를 입력받을 수 있도록 함
