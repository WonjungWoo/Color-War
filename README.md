# Color War
: npc들에 색깔을 칠해 편을 늘려가는 2인용 영역 넓히기, 마피아 게임

##팀원
한양대학교 김선오
KAIST 우원정

##게임 방법
1. 게임을 시작하면 15명의 npc와 플레이어 2명이 필드로 소환된다
2. 플레이어들은 npc들 사이에 숨어서 활동하게 된다
3. 플레이어는 npc에게 자신의 색깔을 칠해 영역을 넓힐 수 있다
4. 과반수의 npc를 칠하면 게임이 끝나게 된다
6. 플레이어는 상대방 플레이어를 찾아 먹어 게임을 끝낼 수 있다


###개발 스펙
- 유니티 (C#)
- Mysql
- flask


![KakaoTalk_Video_2024-01-10-19-50-42-ezgif com-video-to-gif-converter](https://github.com/WonjungWoo/Color-War/assets/37200748/c0e87d40-46a4-46ee-81eb-0e998065f328)

##1.0 로딩 화면
###1.1 스플래시 화면
- Unity 기본 스플래시 애니메이션 활용

![image](https://github.com/WonjungWoo/Color-War/assets/37200748/5e9d9c5c-b3e1-4dee-8e85-26077d0020ca)

###1.2. 로그인 화면
- 카카오 sdk 활용
- access token으로 코틀린 통해 카카오 id 요청

![image](https://github.com/WonjungWoo/Color-War/assets/37200748/bb7a94b4-0f13-4d0c-9608-c27bdd698fab)

###1.3. 닉네임 입력 화면
- 게임에서 사용할 닉네임 입력
- 서버 통한 중복 확인 후 저장



##2.0 메인 화면


![image](https://github.com/WonjungWoo/Color-War/assets/37200748/5aa2ccfd-57af-40a2-8c91-406dfe573eb7)

###2.1. 홈 화면
- Enter 버튼을 통해 방 목록으로 이동 가능
- 랭킹 버튼과 옵션 버튼


![image](https://github.com/WonjungWoo/Color-War/assets/37200748/7ef047db-e859-4592-b99e-fae24aa837f5)

###2.2. 방 목록 화면
- DB에 방 이름과 비밀번호 요청 후 scroll view 생성
- Join 버튼을 통해 로비로 이동 가능
- 비밀번호가 있는 방은 자물쇠 이미지로 표시
- 비밀번호 여부에 따라 비밀번호 입력 페이지로 이동 혹은 로비로 이동
- 우측 상단 + 버튼을 통해 방 생성 가능
- 좌측 상단 홈 버튼을 통해 홈 화면으로 이동 가능



###2.3. 방 비밀번호 입력 화면
- 2.2에서 불러온 비밀번호와 입력 비밀번호 비교
- 잘못된 비밀번호 입력시 wrong input 출력
- 올바른 비밀번호 입력시 로비 페이지로 이동


![image](https://github.com/WonjungWoo/Color-War/assets/37200748/f65e7bc5-8a71-41c4-b1b9-ca0bd7f01f00)

###2.4. 방 생성 화면
- 방 이름과 비밀번호 설정 가능
- DB를 통한 방 목록 관리
- 검색버튼을 통한 방 목록 리로드 및 검색

##3.0 게임 화면


![image](https://github.com/WonjungWoo/Color-War/assets/37200748/1a74ab4e-ceef-4dcf-b870-7f9f489aa53a)
![image](https://github.com/WonjungWoo/Color-War/assets/37200748/1e5f4b49-07ae-4186-bb1f-53f97ce56e64)

###3.1. 로비 화면
- 플레이어 모두 입장 시 색깔 선택 가능
- 각 플레이어마다 준비 버튼 존재
- 모든 플레이어가 준비 버튼을 누르면 시작 버튼 생성
- 시작 버튼을 누르면 본 게임 화면으로 이동 가능

  

![KakaoTalk_Video_2024-01-10-19-53-04-ezgif com-video-to-gif-converter](https://github.com/WonjungWoo/Color-War/assets/37200748/fef0cbef-4067-45cd-a6e7-c785cbf7540e)

###3.2. 본 게임 화면
- 플레이어 2명과 npc 15명 생성
- Photon 서버를 이용한 위치 동기화
- 플레이어의 화면에서는 플레이어를 제외 하얀색으로 세팅
- 조이 스틱을 통한 조작
- 뛰기 버튼, 먹기 버튼, 색칠 버튼 구현
- 과반수의 npc 색칠, 또는 상대방 먹기를 통한 게임 종료



##추후 개발 예정
1. 킬 1회 제한 및 실패시 페널티: 현장 위치 공개
2. 색칠하기 쿨타임 설정
3. 랭킹 기능
4. 조작방법 편집 기능
5. 색 공개를 통한 복지 시스템
