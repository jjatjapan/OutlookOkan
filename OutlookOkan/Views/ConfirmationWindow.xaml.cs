﻿using OutlookOkan.Types;
using OutlookOkan.ViewModels;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookOkan.Views
{
    public partial class ConfirmationWindow : Window
    {
        private readonly Outlook._MailItem _mailItem;

        public ConfirmationWindow(CheckList checkList, Outlook._MailItem mailItem)
        {
            DataContext = new ConfirmationWindowViewModel(checkList);
            _mailItem = mailItem;

            InitializeComponent();

            //送信遅延時間を表示(設定)欄に入れる。
            DeferredDeliveryMinutesBox.Text = checkList.DeferredMinutes.ToString();
        }

        /// <summary>
        /// DialogResultをBindしずらいので、コードビハインドで。
        /// </summary>
        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            //送信時刻の設定
            _ = int.TryParse(DeferredDeliveryMinutesBox.Text, out var deferredDeliveryMinutes);

            if (deferredDeliveryMinutes != 0)
            {
                if (_mailItem.DeferredDeliveryTime == new DateTime(4501, 1, 1, 0, 0, 0))
                {
                    //アドインの機能のみで保留時間が設定された場合
                    _mailItem.DeferredDeliveryTime = DateTime.Now.AddMinutes(deferredDeliveryMinutes);
                }
                else
                {
                    //アドインの機能と同時に、Outlookの標準機能でも保留時間(配信タイミング)が設定された場合
                    if (DateTime.Now.AddMinutes(deferredDeliveryMinutes) > _mailItem.DeferredDeliveryTime.AddMinutes(deferredDeliveryMinutes))
                    {
                        //[既に設定されている送信予定日時+アドインによる保留時間] が [現在日時+アドインによる保留時間] より前の日時となるため、後者を採用する。
                        _mailItem.DeferredDeliveryTime = DateTime.Now.AddMinutes(deferredDeliveryMinutes);
                    }
                    else
                    {
                        _mailItem.DeferredDeliveryTime = _mailItem.DeferredDeliveryTime.AddMinutes(deferredDeliveryMinutes);
                    }
                }
            }

            DialogResult = true;
        }

        /// <summary>
        /// DialogResultをBindしずらいので、コードビハインドで。
        /// </summary>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// チェックボックスのイベント処理がやりづらかったので、コードビハインド側からViewModelのメソッドを呼び出す。
        /// </summary>
        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        /// <summary>
        /// チェックボックスのイベント処理がやりづらかったので、コードビハインド側からViewModelのメソッドを呼び出す。
        /// </summary>
        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        /// <summary>
        /// 送信遅延時間の入力ボックスを数値のみ入力に制限する。
        /// </summary>
        private void DeferredDeliveryMinutesBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+$");
            if (!regex.IsMatch(DeferredDeliveryMinutesBox.Text + e.Text)) return;

            DeferredDeliveryMinutesBox.Text = "0";
            e.Handled = true;
        }

        /// <summary>
        /// 送信遅延時間の入力ボックスへのペーストを無視する。(全角数字がペーストされる恐れがあるため)
        /// </summary>
        private void DeferredDeliveryMinutesBox_OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        #region MouseUpEvent_OnHandler

        private void AlertGridMouseUpEvent_OnHandler(object sender, MouseButtonEventArgs e)
        {
            //左クリック以外は無視する。(CurrentItemがずれる場合があるため)
            if (e.ChangedButton != MouseButton.Left) return;

            var currentItem = (Alert)AlertGrid.CurrentItem;
            currentItem.IsChecked = !currentItem.IsChecked;
            AlertGrid.Items.Refresh();

            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        private void ToGridMouseUpEvent_OnHandler(object sender, MouseButtonEventArgs e)
        {
            //左クリック以外は無視する。(CurrentItemがずれる場合があるため)
            if (e.ChangedButton != MouseButton.Left) return;

            var currentItem = (Address)ToGrid.CurrentItem;
            currentItem.IsChecked = !currentItem.IsChecked;
            ToGrid.Items.Refresh();

            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        private void CcGridMouseUpEvent_OnHandler(object sender, MouseButtonEventArgs e)
        {
            //左クリック以外は無視する。(CurrentItemがずれる場合があるため)
            if (e.ChangedButton != MouseButton.Left) return;

            var currentItem = (Address)CcGrid.CurrentItem;
            currentItem.IsChecked = !currentItem.IsChecked;
            CcGrid.Items.Refresh();

            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        private void BccGridMouseUpEvent_OnHandler(object sender, MouseButtonEventArgs e)
        {
            //左クリック以外は無視する。(CurrentItemがずれる場合があるため)
            if (e.ChangedButton != MouseButton.Left) return;

            var currentItem = (Address)BccGrid.CurrentItem;
            currentItem.IsChecked = !currentItem.IsChecked;
            BccGrid.Items.Refresh();

            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        private void AttachmentGridMouseUpEvent_OnHandler(object sender, MouseButtonEventArgs e)
        {
            //左クリック以外は無視する。(CurrentItemがずれる場合があるため)
            if (e.ChangedButton != MouseButton.Left) return;

            var currentItem = (Attachment)AttachmentGrid.CurrentItem;
            currentItem.IsChecked = !currentItem.IsChecked;
            AttachmentGrid.Items.Refresh();

            var viewModel = DataContext as ConfirmationWindowViewModel;
            viewModel?.ToggleSendButton();
        }

        #endregion

    }
}