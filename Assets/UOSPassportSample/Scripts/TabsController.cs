using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Unity.Passport.Sample.Scripts
{
    public class TabsController : MonoBehaviour
    {
        [Tooltip("tabs的父对象")]
        public Transform tabs;
        [Tooltip("panels的父对象")]
        public Transform panels;

        [Tooltip("是否手动调用 InitTabs 来动态初始化面板")]
        public bool dynamicInit = true;
        
        private int _currentTabIndex;
        private readonly List<GameObject> _panelGameObjects = new();
        private readonly List<TabController> _tabControllers = new();

        public UnityEvent<int> onTabSelect = new();

        private void Start()
        {
            if (!dynamicInit)
            {
                InitTabs();
            }
        }
        public void InitTabs()
        {
            // 先清空
            _panelGameObjects.Clear();
            _tabControllers.Clear();
            _currentTabIndex = 0;
            
            var tabCount = tabs.childCount;
            for (int i = 0; i < tabCount; i += 1)
            {
                var tab = tabs.GetChild(i);
                var panel = panels.GetChild(i);
                
                var index = i;
                tab.gameObject.GetComponent<Button>().onClick.AddListener(() =>
                    TabSelectHandler(index));
                // 设置面板状态并添加近列表
                var panelObj = panel.gameObject;
                panelObj.SetActive(i == 0);
                _panelGameObjects.Add(panelObj);
                
                // 监听 tab 点击事件并初始化状态
                var tabController = tab.GetComponent<TabController>();
                tabController.SetStatus(i == 0);
                _tabControllers.Add(tabController);
            }
        }
        
        private void TabSelectHandler(int index)
        {
            if (index == _currentTabIndex) return;
            // deactivate 上一个
            _panelGameObjects[_currentTabIndex].SetActive(false);
            _tabControllers[_currentTabIndex].SetStatus(false);

            // activate 这一个
            _panelGameObjects[index].SetActive(true);
            _tabControllers[index].SetStatus(true);
            _currentTabIndex = index;
            onTabSelect.Invoke(index);
        }
    }
}