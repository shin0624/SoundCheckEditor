# SoundCheckEditor
- Unity Engine 내에서 필요한 사운드를 즉시 Freesound에서 검색하여 플레이할 수 있는 커스텀에디터
- Freesound : 저작권 무료 음원사이트. (https://freesound.org/)
- API 문서는 http://www.freesound.org/docs/api/ 에서 찾을 수 있음.
- API 키는 https://www.freesound.org/apiv2/apply/ 에서 신청 가능 .

# 패키지 다운로드 링크
https://drive.google.com/file/d/1F8qJMUQgJxh399z4ZgxFmvdOQGWK41ek/view?usp=drive_link

# 개발 환경 
- Unity Engine 2023.2.16f1
- Visual Studio Community 2022
- C#

# 기술 스택
- Unity Editor의 커스텀 에디터 기능
- EditorWindow
- GUILayer
- Freesound APIv2 : Freesound의 api키를 발급받아 사용
- Newtonesoft Json : json데이터 처리를 위해 사용
- System.Net.Http : HttpClient 사용을 위함
- System.Threading.Tasks : 비동기 작업 처리를 위함

# 개요
1. 게임에는 많은 사운드가 필요한데, 작업할 때 마다 필요한 사운드를 찾는 과정이 너무 번거로웠다. AssetStore처럼, 음원 사이트도 에디터 내에서 접근할 수 있으면 편리성이 증대할 것이라 판단하여 사운드 체크 기능을 구현하였다.

# 사용 시 유의
- Json 데이터 처리를 위해 Newtonsoft json 3.2.1버전을 임포트해야 하는데, 오류가 발생하는 경우가 존재. 이럴 때는 asset폴더 내 plugin 폴더에 직접 Newtonsoft.json dll파일을 넣어주면 해결
(패키지 매니저 이름으로 찾기에서 com.unity.nuget.newtonsoft-json 입력 후 3.2.1버전 다운로드) 혹은 구글에 Newtonsoft json 검색 후 다운로드 -> 프로젝트 환경에 dll 추가

# 기능
1. 원하는 사운드를 검색한다. 입력한 쿼리는 Freesound 사이트로 요청되고, 검색 결과가 에디터 창에 출력된다. 원하는 사운드의 play버튼을 클릭하면 Freesound의 해당 사운드 페이지로 연결된다.

# 사용 예시
 ## 다운로드 후, [SoundCheckEditor.cs] 파일에서 본인의 Freesound API KEY 입력(무료. 회원가입 시 발급 가능하며, 분당 60개 요청, 하루 2000개의 사용 제한 존재)
 ![API키요구](https://github.com/user-attachments/assets/2932bfcf-051f-43f3-bf91-73983f71911b)
 
 ## Tool -> SoundCheckEditor 클릭
![toolbar](https://github.com/user-attachments/assets/a34742fa-0c4d-4cd1-9a42-17437d878ab5)
![editordisplay](https://github.com/user-attachments/assets/1e594429-d6f3-4221-b9cf-2e19aed243f2)

 ## Search Sound Effect의 텍스트필드에 검색하고싶은 사운드 명 입력 후 Search 클릭(딜레이 약간 존재)
 ![검색](https://github.com/user-attachments/assets/9d036d0c-5ea5-41f2-b2a0-e5e24dc40bd6)

 ## 검색된 사운드 목록이 출력됨. (페이지 넘기는 기능 및 썸네일 기능 추가 예정)
 ![결과](https://github.com/user-attachments/assets/26f9b380-c94c-4ba9-9da0-42895b3757f6)

 ## 목록 중 원하는 사운드 Play 클릭 시 Freesound의 해당 음향효과 검색 결과창으로 이동
 ![사이트연결](https://github.com/user-attachments/assets/8de394a9-ff07-4bf4-b278-fde511178719)

 ## 검색 성공 시 콘솔 출력
 ![검색결과이상무](https://github.com/user-attachments/assets/05e05035-1db0-48c4-94b1-aed10cc31029)

# 패키지 구성
![스크립트목록소개](https://github.com/user-attachments/assets/08f7cf70-f731-4c2c-85b8-e1e4c2a48779)
 1. FreesoundSearchResult : Freesound API의 검색 결과를 표현할 클래스
 2. FreesoundSound : Freesound API의 사운드 항목을 표현할 클래스
 3. SoundResult : 검색 결과를 표현할 클래스
 4. SoundCheckEditor : 에디터 구현 및 JSON 파싱

# 추가 예정 기능
1. 검색 목록 페이지 넘기는 기능
2. 검색 목록 썸네일 기능
3. Unity 내에서 직접 오디오를 재생하는 기능
4. 에디터 윈도우에서 사이트 연결 없이 바로 미리듣기 기능

# 업데이트 내역
- 2024.09.04 Ver.1
 
