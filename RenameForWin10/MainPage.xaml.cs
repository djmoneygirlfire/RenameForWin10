using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace RenameForWin10
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<string> _oldlist = new List<string>();
        private List<string> _newlist = new List<string>();


        IReadOnlyList<StorageFile> _fileList;
        Windows.Storage.StorageFolder _folder;

        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            speechAI.Text = e.ClickedItem.ToString();

            if (speechAI.Text.Contains("폴더 선택"))
            {
                var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");

                _folder = await folderPicker.PickSingleFolderAsync();
                if (_folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder
                    // (including other sub-folder contents)
                    Windows.Storage.AccessCache.StorageApplicationPermissions.
                    FutureAccessList.AddOrReplace("PickedFolderToken", _folder);
                    speechAI.Text = "Picked folder: " + _folder.Path;

                    _fileList = await _folder.GetFilesAsync();


                    FolderFileList.Items.Clear();

                    if (_fileList.Count % 2 != 0)
                    {
                        speechAI.Text = "폴더 내의 파일이 짝수가 아닙니다.";

                        return;
                    }

                    string 현재표시할라인;

                    _oldlist.Clear();
                    _newlist.Clear();


                    for (int i = 0; i < _fileList.Count; i++)
                    {
                        현재표시할라인 = _fileList[i].Name + " > RN" + string.Format("{0:0000}", Convert_HP_ScanedName(i, _fileList.Count)) + ".jpg";

                        FolderFileList.Items.Add(현재표시할라인);

                        _oldlist.Add(_fileList[i].Path);
                        _newlist.Add("RN" + string.Format("{0:0000}", Convert_HP_ScanedName(i, _fileList.Count)) + ".jpg");
                        

                    }


                }
                else
                {
                    speechAI.Text = "Operation cancelled.";
                }
            }
            else if(speechAI.Text.Contains("이름 바꾸기"))
            {

                //Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", _folder);


                for (int i = 0; i < _oldlist.Count; i++)
                {
                    await _fileList[i].CopyAsync(_folder, _newlist[i], NameCollisionOption.ReplaceExisting);
                }
            }

        }

        int Convert_HP_ScanedName(int nIndex, int count)
        {
            nIndex++;

            int m;
            m = count / 2;

            if (nIndex > m)
            {
                int n;
                n = count - nIndex + 1;
                return 2 * n;
            }
            else
            {
                return 2 * nIndex - 1;
            }
        }
    }


}