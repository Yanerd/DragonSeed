# DragonSeed
11.01 19:47 천지윤
맵 팬스 트리 콜라이더 수정(오브젝트 탈출 막음), 오펜스,디펜스 신 종료 ui수정 (back버튼 누르면 신 전환)
디펜스 신은 수정 필요 (로드 잘 되는지 확인 필요)

11.02 16:37 천지윤 
디펜스 신의 DefenseBattleUiManager 의 endUi의 back 버튼 이벤트 호출 되지 않는 오류 수정 (back버튼 누르면 신 전환 됨)
슬라이더의 렉트 트렌스폼 오류를 고치기 위해 전체 프리팹 자체에 슬라이더를 넣는 작업을 수행할 예정 

11.02 19:14 천지윤
드래곤, 식물 프리팹 슬라이더 수정 캔버스를 프리팹에 넣음 수정 후 생긴 슬라이더 밸류 문제 수정 

11.03 14:00 천지윤&금혜성
우물 슬라이더 밸류 기능되지 않는 오류 수정 
오브젝트 캔버스가 오펜스,디펜스 신에서는 보이지 않도록 수정 
게임 엔드 유아이의 백버튼을 누를 시 두번 로드되는 오류 수정 
게임 엔드 유아이의 백버튼 잔 버그 수정

11.03 19:45 천지윤&금혜성
드래곤 플레이어 추적 함수 fix -> 코루틴 사용
플레이어 체력 동기화 -> photon 예외 설정, 플레이어 데미지 전달함수 재정립
플레이어가 죽으면 타임스케일이 멈추지 않는 오류 수정 
드래곤이 플레이어가 죽어도 공격하는 오류 수정 

11.04 12:34 금혜성
Fence & Tree 에셋 수정 
Scene별 프리팹 적용 
가드닝 Slider nullexception fix

11.4 15:41 금혜성
Post Processing 적용
카메라 이동 함수 수정 적용

11.4 15:41 이원혁
맵 범위 내(defense ui manager에서 mapstate를 받아서 계산 - map state 구매시 범위확장)에서 기즈모 생성, 드래곤이 타겟 오브젝트방향으로 회전하는 값을 증가시켜 빙빙 도는 현상제거 전체적으로 자연스러운 움직임 구현 및 타겟오브젝트 지속시간 증가로 드래곤이 중간중간 멈추는것 수정

11.08 9:10 천지윤
슬라이더 밸류 세이브 기능 추가 (vegetable Awake 단계에서 함수 현재 Scene에 맞게 밸류값 적용 후 생성)
그라운드 인스트 경우에는 기존 그라운드는 생성되어있는 그라운드의 위치를 변경시키는 로직에서 
인스턴시에이트를 하는 로직으로 변경하거나 포톤뷰, 트랜스폼을 넣어서 동기화 해야할 거 같음
SearchRoom 버튼을 누르고 다시 원래 상태로 돌아가면 방향키에따라 우물 슬라이더 밸류가 변경되는 오류가 있음 

11.08 16:20 이원혁
드래곤 랜덤 애니메이션 재생하는 코루틴 제작중

11.08 21:20 천지윤 & 김소라 
Ground State에 따른 fence,tree 생성 구현 중 (Gardening,offence Scene 기존 설치된 fence,tree 삭제 후 신 전환 시 생성하는 로직으로 변경)
포톤 뷰,트랜스폼 뷰를 컴포넌트 추가를 해서 동기화를 하는데, 생성은 동기화가 되지만 위치옮기는 거나 비활성화하는 시점이 달라서인지 동기화가 되지 않음
그래서 맵 스테이트에 맞게 프리팹 제작 후 생성 예정

11.09 19:02 천지윤 & 금혜성
mapSate에 따른 prefab 제작, 맵 level load 시 생성 로직 변경, 상점 그라운드 스테이트 업그레이드 생성 로직 변경
saveLoadManager init로드 시 파일이 없으면 파일 생성 후 기본 구성 지급, 맵정보 세이브 다음 로드 하는 로직으로 변경 
게임을 처음 실행하면 돈 500원, 집1개 우물1개 감자1개를 지급함// data 클래스 변수에 변수를 초기화 후 save하는 형식은 기존 save 함수에서 인잇로드 시 필요한 과정만 하는
save함수를 새로 만듬

11.10 21:00 천지윤 & 금혜성 
플레이어 프리팹 ui와 카메라를 합친 프리팹으로 새로 제작 -플레이어 생성시 카메라를 찾지 못하는 오류가있어 애초에 자식객체로 만듬, 자신일때만 카메라가 활성화되도록함
플레이어 웨폰 콜라이더가 활성화 된상태가 꺼지지 않는 오류가 있어 rpc로 비활성화로 만듬
드래곤 체력바 델리게이트 수정중

11.15 천지윤 & 금혜성 
머드 콜라이더 레이어 수정
팬스 프리팹 콜라이더 추가 (오브젝트 탈출 막기 및 바닥 상태 변경 시 필요) 
디펜스유아이에서 텍스트의 시드카운트가 현재값으로 할당되지 않는 버그 고침  
레이클릭 스크립트에서 오브젝트가 설치 될 경우 시드카운트를 깎고, 0개가 될 경우 버튼 비활성화
배지터블 스크립터블 오브젝트 적용
오펜스신 디펜스 유저 스킬 빛 기둥 구현 (포톤인스턴시,)
플레이어 트랜스퍼 데미지 구현 중 
