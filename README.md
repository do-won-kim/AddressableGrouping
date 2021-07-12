# AddressableGrouping
## 어드레서블 그룹을 자동으로 세팅해주는 툴입니다.  
![image00](https://user-images.githubusercontent.com/73415970/125284851-ef828b00-e354-11eb-83de-fdc559b3eeec.PNG)
---
SettingGroup : 설정값을 가져올 그룹(새로운 그룹을 만들 때 기본 설정 값으로 사용할 그룹입니다.)  
AddGroup : SelectAddressable를 실행 할 때 사용할 에셋들이 들어있는 그룹입니다.  
Limits : 각 그룹별로 들어갈 번들의 갯수 입니다.(스프라이트의 경우 아틀라스가 되어 있지않으면 갯수대로 되어 있으면 아틀라스 별로 묶임)  
  
## Button  
AllAddressable : 모든 어드레서블 그룹에 있는 에셋들을 정리합니다.  
SelectAddressable : AddGroup에 있는 에셋들을 정리합니다.  
CheckAddressable : 정의되지 않은 확장자 명을 찾습니다.  

## 예시
![image02](https://user-images.githubusercontent.com/73415970/125286209-826ff500-e356-11eb-8d1a-b2c783ab80db.PNG)  
툴 돌리기 전
  
![image03](https://user-images.githubusercontent.com/73415970/125286237-8b60c680-e356-11eb-99ac-86aef0a2ccab.PNG)  
돌린 후  

---  
---  
##추가사항  
![image04](https://user-images.githubusercontent.com/73415970/125286464-cbc04480-e356-11eb-9924-e5a21a251601.PNG)  
  
확장자명의 경우 하드 코딩이 되어 있기 때문에 없는 확장자가 발생할 수 있습니다.  
툴 동작 전에 CheckAddressable을 동작해주시면 코드에 없는 확장자명이 있을 경우 Log에 나오니  
확인 하고 추가 해주시면 됩니다.  


