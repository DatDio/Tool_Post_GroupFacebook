using Leaf.xNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Tool_Facebook.Helper;
using Tool_Facebook.Model;

namespace Tool_Facebook.Controller
{
    public class FacebookAPIController
    {
        List<string> _listNotDupilicate;
        //public Random random;
        SqlController SqlController;
        public FacebookAPIController()
        {
            SqlController = new SqlController();
        }
        public ResultModel ScanGroups(AccountModel account, string keyWords)
        {
            _listNotDupilicate = new List<string>(File.ReadAllLines("input/_listNotDupilicate.txt"));
            List<GroupModel> listGroupModel = new List<GroupModel>();
            //int countgroups = 0;
            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.UserAgent = account.C_UserAgent;
                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                //rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                string body = "", refer = "", cursor = "";
                FunctionHelper.AddHeaderxNet(rq, @"dpr: 1.309999942779541
                                                viewport-width: 770
                                                sec-ch-ua: ""Google Chrome"";v=""119"", ""Chromium"";v=""119"", ""Not?A_Brand"";v=""24""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Google Chrome"";v=""119.0.6045.160"", ""Chromium"";v=""119.0.6045.160"", ""Not?A_Brand"";v=""24.0.0.0""
                                                sec-ch-prefers-color-scheme: light
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    //body = rq.Get($"https://www.facebook.com/search/groups?q={keyWords.Replace(" ", "%20")}&filters=eyJwdWJsaWNfZ3JvdXBzOjAiOiJ7XCJuYW1lXCI6XCJwdWJsaWNfZ3JvdXBzXCIsXCJhcmdzXCI6XCJcIn0ifQ%3D%3D").ToString();
                    body = rq.Get($"https://www.facebook.com/search/groups?q={keyWords}").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                var av = RegexHelper.GetValueFromGroup("__user=(.*?)&", body);
                var _user = av;
                var fb_dtsg = RegexHelper.GetValueFromGroup("\"token\":\"(.*?)\"},", body);
                var jazoest = RegexHelper.GetValueFromGroup("jazoest=(.*?)\",", body);
                var lsd = RegexHelper.GetValueFromGroup("\"LSD\":{\"token\":\"(.*?)\"},", body);
                var _spin_t = RegexHelper.GetValueFromGroup("\"__spin_t\":(.*?),", body);
                var _spin_r = RegexHelper.GetValueFromGroup("data-btmanifest=\"(.*?)_main\"", body);
                cursor = RegexHelper.GetValueFromGroup("\"end_cursor\":\"(.*?)\"}}}},", body);
                var matches = Regex.Matches(body, "__isNode\":\"Group\",\"id\":\"(.*?)\",\"__isEntity\":\"Group\",\"profile_url\":\"(.*?)\",\"url\":\"(.*?)\",\"name\":\"(.*?)\",");
                var matches2 = Regex.Matches(body, "\"viewer_forum_join_state\":\"(.*?)\",");
                var matches3 = Regex.Matches(body, "\"prominent_snippet_text_with_entities\":null,\"primary_snippet_text_with_entities\":{\"delight_ranges\":\\[],\"image_ranges\":\\[],\"inline_style_ranges\":\\[],\"aggregated_ranges\":\\[],\"ranges\":\\[],\"color_ranges\":\\[],\"text\":\"(.*?) \\\\u00b7 (.*?) ");
                if (matches.Count == 0) return ResultModel.Fail;
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    if (match.Success)
                    {
                        GroupModel groupModel = new GroupModel();
                        groupModel.C_UIDGroup = match.Groups[1].Value;
                        groupModel.C_NameGroup = (match.Groups[4].Value).ToString();
                        if (groupModel.C_UIDGroup != "")
                        {
                            if (_listNotDupilicate.Contains(groupModel.C_UIDGroup))
                            {
                                continue;
                            }
                            _listNotDupilicate.Add(groupModel.C_UIDGroup);
                            File.AppendAllText("input/_listNotDupilicate.txt", groupModel.C_UIDGroup + "\n");
                            if (matches2.Count == matches.Count && matches2[i].Groups[1].Value == "CAN_JOIN")
                            {
                                groupModel.C_TypeGroup = "Công khai";
                            }
                            else
                            {
                                groupModel.C_TypeGroup = "Riêng tư";
                            }

                            var _censorship = CheckCensorship(account, groupModel.C_UIDGroup);
                            if (_censorship == ResultModel.CheckPoint)
                            {
                                FunctionHelper.EditValueColumn(account, "C_Status", "Login bị checkpoint", true);
                            }
                            else if (_censorship == ResultModel.KiemDuyet)
                            {
                                groupModel.C_Censorship = "Kiểm duyệt";
                            }
                            else if (_censorship == ResultModel.KoKiemDuyet)
                            {
                                groupModel.C_Censorship = "Không kiểm duyệt";
                            }
                            if (matches3.Count == matches.Count)
                            {
                                groupModel.C_MemberGroup = FunctionHelper.ConvertToInt(matches3[i].Groups[2].Value).ToString("N0");
                            }

                            groupModel.C_FolderGroup = Form1._cbbFolderManageGroup;
                            groupModel.C_IDGroup = Guid.NewGuid().ToString();
                            listGroupModel.Add(groupModel);
                        }
                    }
                }
                SqlController.BulkInsert(listGroupModel);
                SqlController.ReloadDataFolderManageGroup(listGroupModel);
                listGroupModel.Clear();
                while (cursor != "" && !Form1.stop)
                {
                    //Cuộn xuống để lấy group tiếp
                    FunctionHelper.AddHeaderxNet(rq, $@"sec-ch-ua: ""Google Chrome"";v=""119"", ""Chromium"";v=""119"", ""Not?A_Brand"";v=""24""
                                                sec-ch-ua-mobile: ?0
                                                viewport-width: 770
                                                X-FB-Friendly-Name: SearchCometResultsPaginatedResultsQuery
                                                X-FB-LSD: {lsd}
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                Content-Type: application/x-www-form-urlencoded
                                                X-ASBD-ID: 129477
                                                dpr: 1.31
                                                sec-ch-ua-model: """"
                                                sec-ch-prefers-color-scheme: light
                                                sec-ch-ua-platform: ""Windows""
                                                Accept: */*
                                                Sec-Fetch-Site: same-origin
                                                Sec-Fetch-Mode: cors
                                                Sec-Fetch-Dest: empty
                                                Referer: {refer}");
                    var pr = new RequestParams()
                    {
                        ["av"] = av,
                        ["__user"] = _user,
                        ["__a"] = "1",
                        ["__req"] = "1",
                        ["__hs"] = "19679.HYP:comet_pkg.2.1..2.1",
                        ["dpr"] = "1.5",
                        ["__ccg"] = "EXCELLENT",
                        ["__rev"] = "1009974351",
                        ["__s"] = "lm70d8:8juwhq:y1k1my",
                        ["__hsi"] = "7302660626887442362",
                        ["__dyn"] = "7AzHK4HzE4e5Q1ryaxG4VuC2-m1xDwAxu13wFwhUngS3q5UObwNwnof8boG0x8bo6u3y4o2Gwfi0LVEtwMw65xO2OU7m2210wEwgolzUO0-E7m4oaEnxO0Bo7O2l2Utwwwi831w9O7Udo5qfK0zEkxe2GewyDwsoqBwJK2W5olwUwOzEjUlwhEe88o4Wm7-8wywdG7FoarCwLyESE6C14wwwOg2cwMwrUdUcojxK2B0oobo8oC1Iwqo4e16wWw-zXDw",
                        ["__csr"] = "3pqIz8ZrqTWENeK_kQBt-YCD9QSWlqiKUFiAvt7iaBniFi8JddnZ9F4CVe_kW8GmAGJ5amkDJ2EyviJEx4KSKkEghpQqih9rWG9XFa8BDGm7azAFQqVFHBCG_Cz8Ly8CGyFoyt34V8-bxt2V8W9hUS4rojyWJeA5Ua8oyQ5oy9DF1GcAyEy5opCACwyx213zO2o7OdxafypXJ7wwwhk698aazoaEfA2e68CufxF7wNxmUhyLzE9E5m2eq7oO4oc8Su1cK1HxS7okDwwCwwwHx-15wrawMwQwsU5-qh04ow7Rx-0xo3Aw9Gm0mq09Yweq0hG15wlQ10xq1awrpFUrU04u205dA05ZE07oq03nq0yo0V503fo0zC0iR0jU7y0qG0lm0rG08cAt06pQ1Aw0WZw2kF8mw1O60ue02-e0NC0iu04M83nwWw54w76g0Lt0dQVHwzgbEhQ",
                        ["__comet_req"] = "15",
                        ["fb_dtsg"] = fb_dtsg,
                        ["jazoest"] = jazoest,
                        ["lsd"] = lsd,
                        ["__aaid"] = "0",
                        ["__spin_r"] = _spin_r,
                        ["__spin_b"] = "trunk",
                        ["__spin_t"] = _spin_t,
                        ["qpl_active_flow_ids"] = "1056842055",
                        ["fb_api_caller_class"] = "RelayModern",
                        ["fb_api_req_friendly_name"] = "SearchCometResultsPaginatedResultsQuery",
                        ["variables"] = $"{{\"UFI2CommentsProvider_commentsKey\":\"SearchCometResultsInitialResultsQuery\",\"allow_streaming\":false,\"args\":{{\"callsite\":\"COMET_GLOBAL_SEARCH\",\"config\":{{\"exact_match\":false,\"high_confidence_config\":null,\"intercept_config\":null,\"sts_disambiguation\":null,\"watch_config\":null}},\"context\":{{\"bsid\":\"b5e93807-fc39-4d03-897c-5a9e99eaa892\",\"tsid\":null}},\"experience\":{{\"encoded_server_defined_params\":null,\"fbid\":null,\"type\":\"GROUPS_TAB\"}},\"filters\":[\"{{\\\"name\\\":\\\"public_groups\\\",\\\"args\\\":\\\"\\\"}}\"],\"text\":\"shopee\"}},\"count\":5,\"cursor\":\"{cursor}\",\"displayCommentsContextEnableComment\":false,\"displayCommentsContextIsAdPreview\":false,\"displayCommentsContextIsAggregatedShare\":false,\"displayCommentsContextIsStorySet\":false,\"displayCommentsFeedbackContext\":null,\"feedLocation\":\"SEARCH\",\"feedbackSource\":23,\"fetch_filters\":true,\"focusCommentID\":null,\"locale\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"search_results_page\",\"scale\":1.5,\"stream_initial_count\":0,\"useDefaultActor\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIIsRTAEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true,\"__relay_internal__pv__StoriesRingrelayprovider\":true}}",
                        ["server_timestamps"] = "true",
                        ["doc_id"] = "6954151207975822",
                    };
                    try
                    {
                        body = rq.Post("https://www.facebook.com/api/graphql/", pr).ToString();
                        refer = rq.Address.AbsoluteUri;

                    }
                    catch
                    {
                        return ResultModel.Fail;
                    }
                    cursor = RegexHelper.GetValueFromGroup("\"end_cursor\":\"(.*?)\"}}}},", body);
                    matches = Regex.Matches(body, "__isNode\":\"Group\",\"id\":\"(.*?)\",\"__isEntity\":\"Group\",\"profile_url\":\"(.*?)\",\"url\":\"(.*?)\",\"name\":\"(.*?)\",");
                    matches2 = Regex.Matches(body, "\"viewer_forum_join_state\":\"(.*?)\",");
                    matches3 = Regex.Matches(body, "\"prominent_snippet_text_with_entities\":null,\"primary_snippet_text_with_entities\":{\"delight_ranges\":\\[],\"image_ranges\":\\[],\"inline_style_ranges\":\\[],\"aggregated_ranges\":\\[],\"ranges\":\\[],\"color_ranges\":\\[],\"text\":\"(.*?) \\\\u00b7 (.*?) ");
                    if (matches.Count == 0) return ResultModel.Fail;
                    for (int i = 0; i < matches.Count; i++)
                    {
                        var match = matches[i];
                        if (match.Success)
                        {
                            GroupModel groupModel = new GroupModel();
                            groupModel.C_UIDGroup = match.Groups[1].Value;
                            groupModel.C_NameGroup = (match.Groups[4].Value).ToString();
                            if (groupModel.C_UIDGroup != "")
                            {
                                if (_listNotDupilicate.Contains(groupModel.C_UIDGroup))
                                {
                                    continue;
                                }
                                _listNotDupilicate.Add(groupModel.C_UIDGroup);
                                File.AppendAllText("input/_listNotDupilicate.txt", groupModel.C_UIDGroup + "\n");
                                if (matches2.Count == matches.Count && matches2[i].Groups[1].Value == "CAN_JOIN")
                                {
                                    groupModel.C_TypeGroup = "Công khai";
                                }
                                else
                                {
                                    groupModel.C_TypeGroup = "Riêng tư";
                                }

                                var _censorship = CheckCensorship(account, groupModel.C_UIDGroup);
                                if (_censorship == ResultModel.CheckPoint)
                                {
                                    FunctionHelper.EditValueColumn(account, "C_Status", "Login bị checkpoint", true);
                                }
                                else if (_censorship == ResultModel.KiemDuyet)
                                {
                                    groupModel.C_Censorship = "Kiểm duyệt";
                                }
                                else if (_censorship == ResultModel.KoKiemDuyet)
                                {
                                    groupModel.C_Censorship = "Không kiểm duyệt";
                                }
                                if (matches3.Count == matches.Count)
                                {
                                    groupModel.C_MemberGroup = FunctionHelper.ConvertToInt(matches3[i].Groups[2].Value).ToString("N0");

                                }

                                groupModel.C_FolderGroup = Form1._cbbFolderManageGroup;
                                groupModel.C_IDGroup = Guid.NewGuid().ToString();
                                listGroupModel.Add(groupModel);
                            }
                        }
                    }
                    SqlController.BulkInsert(listGroupModel);
                    SqlController.ReloadDataFolderManageGroup(listGroupModel);
                    listGroupModel.Clear();
                }
            }
            return ResultModel.Success;
        }
        public ResultModel CheckCensorship(AccountModel account, string groupID)
        {
            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.UserAgent = account.C_UserAgent;
                //account.C_Cookie = "sb=zYiCYXmTV6Lo2j0SSdD4z-EX; datr=BFBXZS3zHKgU7vEYxxtr9uq0; locale=vi_VN; wl_cbv=v2%3Bclient_version%3A2376%3Btimestamp%3A1702437833; vpd=v1%3B659x400x2.0000000509232905; dpr=1.309999942779541; c_user=100088692310375; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1702547252349%2C%22v%22%3A1%7D; wd=798x701; xs=45%3AhxVrLNy5Um6BYQ%3A2%3A1702543845%3A-1%3A-1%3A%3AAcV7Noj7b0WSZO8oCntdpI54kdnU0hZXYebb3VvV5A; fr=10jmntnzueF170soo.AWV4PnA3-QqnKL8REWFz6GxO4xY.BletBJ.KC.AAA.0.0.BletBJ.AWV32Xp4ATY";

                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                string body = "", refer = "";
                //Vào group
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get($"https://d.facebook.com/groups/{groupID}/madminpanel").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                if (refer.Contains("checkpoint")) return ResultModel.CheckPoint;
                if (body.Contains("madminpanel/pending")) return ResultModel.KiemDuyet;
                return ResultModel.KoKiemDuyet;
            }
        }
        public ResultModel CheckLiveUid(AccountModel account)
        {
            FunctionHelper.EditValueColumn(account, "C_Status", $"Đang check live uid!");

            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.57";
                //rq.Proxy = FunctionHelper.ConvertToProxyClient(accountModel.Account.Proxy);

                var body = "";
                try
                {
                    rq.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    rq.AddHeader("Accept-Language", "en-US,en;q=0.9");
                    rq.AddHeader("sec-ch-ua-mobile", "?0");
                    rq.AddHeader("sec-ch-ua-platform", "\"Windows\"");
                    rq.AddHeader("Sec-Fetch-Dest", "document");
                    rq.AddHeader("Sec-Fetch-Mode", "navigate");
                    rq.AddHeader("Sec-Fetch-Site", "none");
                    rq.AddHeader("Sec-Fetch-User", "?1");
                    rq.AddHeader("Upgrade-Insecure-Requests", "1");
                    body = rq.Get($"https://graph.facebook.com/{account.C_UID}/picture?type=normal&redirect=false").ToString();
                    if (body.Contains("height") && body.Contains("width"))
                    {

                        return ResultModel.Success;
                    }
                }
                catch
                {
                    //
                }

            }
            return ResultModel.Fail;
        }
        public ResultModel ShareLinkPost(AccountModel account, GroupModel group, string link, string content)
        {
            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.UserAgent = account.C_UserAgent;
                //account.C_Cookie = "sb=zYiCYXmTV6Lo2j0SSdD4z-EX; datr=BFBXZS3zHKgU7vEYxxtr9uq0; locale=vi_VN; wl_cbv=v2%3Bclient_version%3A2376%3Btimestamp%3A1702437833; vpd=v1%3B659x400x2.0000000509232905; dpr=1.309999942779541; usida=eyJ2ZXIiOjEsImlkIjoiQXM1bDZ5Nzk3Z21hdCIsInRpbWUiOjE3MDI0Mzk3OTF9; c_user=100088692310375; xs=40%3Ast2vRN1IKclxTA%3A2%3A1702462228%3A-1%3A-1; fr=1U5OuF6ARu4HFAffd.AWW5tL9SIynemLngCfmq8ZZ2zXs.BleYCx.KC.AAA.0.0.BleYMU.AWVMWMqDjkw; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1702462239333%2C%22v%22%3A1%7D; wd=646x701";
                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                string body = "", refer = "", postID = "";
                RequestParams pr;
                //Lấy shareID
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get(link).ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                var sharepr = RegexHelper.GetValueFromGroup("\"subscription_target_id\":\"(.*?)\"", body);
                var av = RegexHelper.GetValueFromGroup("__user=(.*?)&", body);
                var _user = av;
                var fb_dtsg = RegexHelper.GetValueFromGroup("\"DTSGInitialData\",\\[\\],{\"token\":\"(.*?)\"}", body);
                var jazoest = RegexHelper.GetValueFromGroup("jazoest=(.*?)\",", body);
                var lsd = RegexHelper.GetValueFromGroup("\"LSD\",\\[\\],{\"token\":\"(.*?)\"}", body);
                //Post
                pr = new RequestParams()
                {
                    ["av"] = av,
                    ["__user"] = _user,
                    ["__a"] = "1",
                    ["__req"] = "17",
                    ["__hs"] = "19679.HYP:comet_pkg.2.1..2.1",
                    ["dpr"] = "1",
                    ["__ccg"] = "EXCELLENT",
                    ["__rev"] = "1010370061",
                    ["__s"] = "ri5myp:vkgg1p:mfn1r5",
                    ["__hsi"] = "7311923747519694665",
                    ["__dyn"] = "7AzHK4HwkEng5K8G6EjBAo2nDwAxu13wFwhUngS3q2ibwNw9G2Saw8i2S1DwUx60GE5O0BU2_CxS320om78bbwto886C11xmfz83WwgEcEhwGxu782lwv89kbxS2218wc61awkovwRwlE-U2exi4UaEW2G1jxS6FobrwKxm5oe8464-5pU9UmwSU8o4Wm7-2K0-poarCwLyESE6C14wwwOg2cwMwhEkxe3u364UrwFg662S269wkopg6C13whEeE4WVufw",
                    ["__csr"] = "g42I9l4i8Qp9bf4bvsAv4RfZR4tTvifq8YkymLkBQCISymAGaxa-ATitaiXRiBAVaJCmVTh9qRRihXHQeA46poVpHyVXWHWVqVFaADAGWWixl2A-uUW5rAyECKl9zkmi8GqVQ4pAES211i9HAG2q4uaQEyu2ai4Wy8iDAxqmi9G4UO4UG69GDx64XwQGbDwIwOKu3a9wh8G22uu12AwIwrogo4l1y2a7ECawKwjE2ewzU4KazUb5US3O8wq8bUf86O7EaE3Vw8i0k6083xm1Vy81-EK0MQ1cg0Avw7bw2m81golhofu0ny01nXw10i00Q1N0do0Ii0PE-0o20qV03hU2kw4KCG9w4aw9S0qG0r-zo1aU0szDQ02Md04kw2E81N8ao0cAE0ajE3eo1iE0lDw1fW",
                    ["__comet_req"] = "15",
                    ["fb_dtsg"] = fb_dtsg,
                    ["jazoest"] = jazoest,
                    ["lsd"] = lsd,
                    ["__aaid"] = "0",
                    ["__spin_r"] = "1010370061",
                    ["__spin_b"] = "trunk",
                    ["__spin_t"] = "1702439912",
                    ["qpl_active_flow_ids"] = "431626709",
                    ["fb_api_caller_class"] = "RelayModern",
                    ["fb_api_req_friendly_name"] = "ComposerStoryCreateMutation",
                    ["variables"] = $"{{\"input\":{{\"composer_entry_point\":\"inline_composer\",\"composer_source_surface\":\"group\",\"composer_type\":\"group\",\"logging\":{{\"composer_session_id\":\"526f7282-3302-4014-8ee1-a37c2ecdc5e1\"}},\"source\":\"WWW\",\"attachments\":[{{\"link\":{{\"share_scrape_data\":\"{{\\\"share_type\\\":22,\\\"share_params\\\":[{sharepr}]}}\"}}}}],\"message\":{{\"ranges\":[],\"text\":\"{content}\"}},\"with_tags_ids\":[],\"inline_activities\":[],\"explicit_place_id\":\"0\",\"text_format_preset_id\":\"0\",\"navigation_data\":{{\"attribution_id_v2\":\"CometGroupDiscussionRoot.react,comet.group,via_cold_start,1702439915702,670176,2361831622,,\"}},\"tracking\":[null],\"event_share_metadata\":{{\"surface\":\"newsfeed\"}},\"audience\":{{\"to_id\":\"{group.C_UIDGroup}\"}},\"actor_id\":\"{_user}\",\"client_mutation_id\":\"1\"}},\"displayCommentsFeedbackContext\":null,\"displayCommentsContextEnableComment\":null,\"displayCommentsContextIsAdPreview\":null,\"displayCommentsContextIsAggregatedShare\":null,\"displayCommentsContextIsStorySet\":null,\"feedLocation\":\"GROUP\",\"feedbackSource\":0,\"focusCommentID\":null,\"gridMediaWidth\":null,\"groupID\":null,\"scale\":1,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"checkPhotosToReelsUpsellEligibility\":false,\"renderLocation\":\"group\",\"useDefaultActor\":false,\"inviteShortLinkKey\":null,\"isFeed\":false,\"isFundraiser\":false,\"isFunFactPost\":false,\"isGroup\":true,\"isEvent\":false,\"isTimeline\":false,\"isSocialLearning\":false,\"isPageNewsFeed\":false,\"isProfileReviews\":false,\"isWorkSharedDraft\":false,\"UFI2CommentsProvider_commentsKey\":\"CometGroupDiscussionRootSuccessQuery\",\"hashtag\":null,\"canUserManageOffers\":false,\"__relay_internal__pv__CometUFIIsRTAEnabledrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesRingrelayprovider\":false}}",
                    ["server_timestamps"] = "true",
                    ["doc_id"] = "7677593048935391",
                    ["fb_api_analytics_tags"] = "[\"qpl_active_flow_ids=431626709\"]",

                };
                FunctionHelper.AddHeaderxNet(rq, $@"sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                            DNT: 1
                                            sec-ch-ua-mobile: ?0
                                            viewport-width: 2510
                                            X-FB-Friendly-Name: ComposerStoryCreateMutation
                                            X-FB-LSD: {lsd}
                                            sec-ch-ua-platform-version: ""15.0.0""
                                            Content-Type: application/x-www-form-urlencoded
                                            X-ASBD-ID: 129477
                                            dpr: 1
                                            sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                            sec-ch-ua-model: """"
                                            sec-ch-prefers-color-scheme: dark
                                            sec-ch-ua-platform: ""Windows""
                                            Accept: */*
                                            Origin: https://www.facebook.com
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: cors
                                            Sec-Fetch-Dest: empty
                                            Referer: {refer}");
                try
                {
                    body = rq.Post($"https://www.facebook.com/api/graphql/", pr).ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                postID = RegexHelper.GetValueFromGroup("\"post_id\":\"(.*?)\",", body);
                if (postID == "")
                {
                    return ResultModel.Fail;
                }
                //Vào link post
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get($"https://www.facebook.com/{postID}").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                if (refer == $"https://www.facebook.com/{postID}")
                    return ResultModel.PostDeleted;
                group.C_PostID = postID;
                group.C_CreatedPost = DateTime.UtcNow.ToString();
                group.C_UIDVia = account.C_UID;
                return ResultModel.Success;
            }
        }
        public ResultModel EditPost(AccountModel account, GroupModel group, string link, string content)
        {
            //postID = "1924394571290660";
            //content = "Dat Dio";
            //link = "https://www.facebook.com/DuyKhanhOfficial/posts/pfbid029nDYMur5L6ypfaF53jWmTvXBY1Ev19nwNu8CgNkfgb2jN17k8KdSZYicA1Sb5dfl";
            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.UserAgent = account.C_UserAgent;
                //account.C_Cookie = "sb=zYiCYXmTV6Lo2j0SSdD4z-EX; datr=BFBXZS3zHKgU7vEYxxtr9uq0; locale=vi_VN; wl_cbv=v2%3Bclient_version%3A2376%3Btimestamp%3A1702437833; vpd=v1%3B659x400x2.0000000509232905; dpr=1.309999942779541; c_user=100088692310375; xs=45%3AhxVrLNy5Um6BYQ%3A2%3A1702543845%3A-1%3A-1; fr=1GICnUX4adUmZeKU5.AWVW0wVSVcD6vSY-Y1W1Q0rR41g.BlesBp.KC.AAA.0.0.BlesHl.AWXYzotFWW0; wd=798x701; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1702544105775%2C%22v%22%3A1%7D";
                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                string body = "", refer = "";
                RequestParams pr;
                //Lấy shareID
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get(link).ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                var sharepr = RegexHelper.GetValueFromGroup("\"subscription_target_id\":\"(.*?)\"", body);
                //Vào link post
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get($"https://www.facebook.com/{group.C_PostID}").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }

                var av = RegexHelper.GetValueFromGroup("__user=(.*?)&", body);
                if (av == "0" || av == "")
                {
                    return ResultModel.Fail;
                }
                var _user = av;
                var fb_dtsg = RegexHelper.GetValueFromGroup("\"DTSGInitialData\",\\[\\],{\"token\":\"(.*?)\"}", body);
                var jazoest = RegexHelper.GetValueFromGroup("jazoest=(.*?)\",", body);
                var lsd = RegexHelper.GetValueFromGroup("\"LSD\",\\[\\],{\"token\":\"(.*?)\"}", body);
                var storyID = RegexHelper.GetValueFromGroup("\"storyID\":\"(.*?)\",", body);
                //Edit

                pr = new RequestParams()
                {
                    ["av"] = av,
                    ["__user"] = _user,
                    ["__a"] = "1",
                    ["__req"] = "18",
                    ["__hs"] = "19679.HYP:comet_pkg.2.1..2.1",
                    ["dpr"] = "1",
                    ["__ccg"] = "EXCELLENT",
                    ["__rev"] = "1010386067",
                    ["__s"] = "hlymz0:b1pqgy:8m8gxj",
                    ["__hsi"] = "7312037056206658765",
                    ["__dyn"] = "7AzHK4HwkEng5K8G6EjBAo2nDwAxu13wFwhUngS3q2ibwNw9G2Saw8i2S1DwUx60GE5O0BU2_CxS320om78bbwto886C11xmfz83WwgEcEhwGxu782lwv89kbxS2218wc61awkovwRwlE-U2exi4UaEW2G1jxS6FobrwKxm5oe8464-5pU9UmwSU8o4Wm7-2K0-poarCwLyESE6C14wwwOg2cwMwhEkxe3u364UrwFg662S269wkopg6C13whEeE4WVU-",
                    ["__csr"] = "ge23kIrHv_lOlERWZPNcAjdvuhpcR9H98IHsJTRNkGFTkIOkySGOXL4auZ9IxqFoOmAH-WGmuBKyrGGhaDGmvDh996l6qVOmuBjyHHAKcDh42S-ihap9ai5qG8ByKmvhooyAGWxhoO5A5awxUjBwEy8CmrwxzqgeUbUB7K4ayUx2UbVaK2u48jwv8cUbrK2udK2i1iwhEdUb9o9ouyVU1co7S7EG3yEowgE9U6q09Zw5yw2N80Ge320py0iq0Qe19g0Sm1qy8053C00_78jgbE0ya0xE2Kw2fE0u6808ew3NoG0aqwfy06JQ0HU0Ca015Mw0Xpw21S0iy02pW0Do",
                    ["__comet_req"] = "15",
                    ["fb_dtsg"] = fb_dtsg,
                    ["jazoest"] = jazoest,
                    ["lsd"] = lsd,
                    ["__aaid"] = "0",
                    ["__spin_r"] = "1010386067",
                    ["__spin_b"] = "trunk",
                    ["__spin_t"] = "1702466294",
                    ["qpl_active_flow_ids"] = "431626709",
                    ["fb_api_caller_class"] = "RelayModern",
                    ["fb_api_req_friendly_name"] = "ComposerStoryEditMutation",
                    ["variables"] = $"{{\"input\":{{\"composer_entry_point\":\"inline_composer\",\"composer_source_surface\":\"group\",\"composer_type\":\"edit\",\"logging\":{{\"composer_session_id\":\"ef18c258-1c04-4dae-917b-eb2fc452f985\"}},\"story_id\":\"{storyID}\",\"attachments\":[{{\"link\":{{\"share_scrape_data\":\"{{\\\"share_type\\\":22,\\\"share_params\\\":[{sharepr}]}}\"}}}}],\"message\":{{\"ranges\":[],\"text\":\"{content}\"}},\"with_tags_ids\":[],\"inline_activities\":[],\"explicit_place_id\":\"0\",\"text_format_preset_id\":\"0\",\"editable_post_feature_capabilities\":[\"CONTAINED_LINK\",\"CONTAINED_MEDIA\",\"POLL\"],\"actor_id\":\"{_user}\",\"client_mutation_id\":\"1\"}},\"displayCommentsFeedbackContext\":null,\"displayCommentsContextEnableComment\":null,\"displayCommentsContextIsAdPreview\":null,\"displayCommentsContextIsAggregatedShare\":null,\"displayCommentsContextIsStorySet\":null,\"feedLocation\":\"GROUP\",\"feedbackSource\":1,\"focusCommentID\":null,\"scale\":1.5,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"group_permalink\",\"useDefaultActor\":false,\"UFI2CommentsProvider_commentsKey\":null,\"isGroupViewerContent\":false,\"isSocialLearning\":false,\"isWorkDraftFor\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIIsRTAEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesRingrelayprovider\":false}}",
                    ["server_timestamps"] = "true",
                    ["doc_id"] = "7272429606174375",
                    ["fb_api_analytics_tags"] = "[\"qpl_active_flow_ids=431626709\"]",

                };
                FunctionHelper.AddHeaderxNet(rq, $@"sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Google Chrome"";v=""120""
                                            sec-ch-ua-mobile: ?0
                                            viewport-width: 746
                                            X-FB-Friendly-Name: ComposerStoryEditMutation
                                            X-FB-LSD: {lsd}
                                            sec-ch-ua-platform-version: ""15.0.0""
                                            Content-Type: application/x-www-form-urlencoded
                                            X-ASBD-ID: 129477
                                            dpr: 1.31
                                            sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Google Chrome"";v=""120.0.6099.71""
                                            sec-ch-ua-model: """"
                                            sec-ch-prefers-color-scheme: light
                                            sec-ch-ua-platform: ""Windows""
                                            Accept: */*
                                            Origin: https://www.facebook.com
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: cors
                                            Sec-Fetch-Dest: empty
                                            Referer: {refer}");
                try
                {
                    body = rq.Post($"https://www.facebook.com/api/graphql/", pr).ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                //Vào link post
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get($"https://www.facebook.com/{group.C_PostID}").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                if (refer.Contains("pending_posts"))
                {
                    return ResultModel.Fail;
                }
                if (refer == $"https://www.facebook.com/{group.C_PostID}")
                    return ResultModel.PostDeleted;
                var newsharepr = RegexHelper.GetValueFromGroup("\"original_content_id\\\\\":\\\\\"(.*?)\\\\\",", body);
                if (newsharepr == sharepr)
                {
                    group.C_TimeEditPost = DateTime.Now.ToString();
                    return ResultModel.Success;
                }
                return ResultModel.Fail;
            }
        }
        public ResultModel PostTextToGroup(AccountModel account, GroupModel group, string content)
        {
            //content = "alo123456789";
            //groupID = "132119656498259";
            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.UserAgent = account.C_UserAgent;

                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                string body = "", refer = "", postID = "";
                RequestParams pr;
                //Vào group
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get($"https://www.facebook.com/groups/{group.C_UIDGroup}").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                if (refer.Contains("checkpoint")) return ResultModel.CheckPoint;
                //var sharepr = RegexHelper.GetValueFromGroup("\"subscription_target_id\":\"(.*?)\"", body);
                var av = RegexHelper.GetValueFromGroup("__user=(.*?)&", body);
                if (av == "0" || av == "")
                {
                    return ResultModel.Fail;
                }
                var _user = av;
                var fb_dtsg = RegexHelper.GetValueFromGroup("\"DTSGInitialData\",\\[\\],{\"token\":\"(.*?)\"}", body);
                var jazoest = RegexHelper.GetValueFromGroup("jazoest=(.*?)\",", body);
                var lsd = RegexHelper.GetValueFromGroup("\"LSD\",\\[\\],{\"token\":\"(.*?)\"}", body);
                //Post
                pr = new RequestParams()
                {
                    ["av"] = av,
                    ["__user"] = _user,
                    ["__a"] = "1",
                    ["__req"] = "17",
                    ["__hs"] = "19679.HYP:comet_pkg.2.1..2.1",
                    ["dpr"] = "1",
                    ["__ccg"] = "EXCELLENT",
                    ["__rev"] = "1010370061",
                    ["__s"] = "ri5myp:vkgg1p:mfn1r5",
                    ["__hsi"] = "7311923747519694665",
                    ["__dyn"] = "7AzHK4HwkEng5K8G6EjBAo2nDwAxu13wFwhUngS3q2ibwNw9G2Saw8i2S1DwUx60GE5O0BU2_CxS320om78bbwto886C11xmfz83WwgEcEhwGxu782lwv89kbxS2218wc61awkovwRwlE-U2exi4UaEW2G1jxS6FobrwKxm5oe8464-5pU9UmwSU8o4Wm7-2K0-poarCwLyESE6C14wwwOg2cwMwhEkxe3u364UrwFg662S269wkopg6C13whEeE4WVufw",
                    ["__csr"] = "g42I9l4i8Qp9bf4bvsAv4RfZR4tTvifq8YkymLkBQCISymAGaxa-ATitaiXRiBAVaJCmVTh9qRRihXHQeA46poVpHyVXWHWVqVFaADAGWWixl2A-uUW5rAyECKl9zkmi8GqVQ4pAES211i9HAG2q4uaQEyu2ai4Wy8iDAxqmi9G4UO4UG69GDx64XwQGbDwIwOKu3a9wh8G22uu12AwIwrogo4l1y2a7ECawKwjE2ewzU4KazUb5US3O8wq8bUf86O7EaE3Vw8i0k6083xm1Vy81-EK0MQ1cg0Avw7bw2m81golhofu0ny01nXw10i00Q1N0do0Ii0PE-0o20qV03hU2kw4KCG9w4aw9S0qG0r-zo1aU0szDQ02Md04kw2E81N8ao0cAE0ajE3eo1iE0lDw1fW",
                    ["__comet_req"] = "15",
                    ["fb_dtsg"] = fb_dtsg,
                    ["jazoest"] = jazoest,
                    ["lsd"] = lsd,
                    ["__aaid"] = "0",
                    ["__spin_r"] = "1010370061",
                    ["__spin_b"] = "trunk",
                    ["__spin_t"] = "1702439912",
                    ["qpl_active_flow_ids"] = "431626709",
                    ["fb_api_caller_class"] = "RelayModern",
                    ["fb_api_req_friendly_name"] = "ComposerStoryCreateMutation",
                    ["variables"] = $"{{\"input\":{{\"composer_entry_point\":\"inline_composer\",\"composer_source_surface\":\"group\",\"composer_type\":\"group\",\"logging\":{{\"composer_session_id\":\"b9251a49-558c-44d7-bd36-16c44e244579\"}},\"source\":\"WWW\",\"attachments\":[],\"message\":{{\"ranges\":[],\"text\":\"{content}\"}},\"with_tags_ids\":[],\"inline_activities\":[],\"explicit_place_id\":\"0\",\"text_format_preset_id\":\"0\",\"navigation_data\":{{\"attribution_id_v2\":\"CometGroupDiscussionRoot.react,comet.group,via_cold_start,1702462821082,977598,2361831622,,\"}},\"tracking\":[null],\"event_share_metadata\":{{\"surface\":\"newsfeed\"}},\"audience\":{{\"to_id\":\"{group.C_UIDGroup}\"}},\"actor_id\":\"{_user}\",\"client_mutation_id\":\"1\"}},\"displayCommentsFeedbackContext\":null,\"displayCommentsContextEnableComment\":null,\"displayCommentsContextIsAdPreview\":null,\"displayCommentsContextIsAggregatedShare\":null,\"displayCommentsContextIsStorySet\":null,\"feedLocation\":\"GROUP\",\"feedbackSource\":0,\"focusCommentID\":null,\"gridMediaWidth\":null,\"groupID\":null,\"scale\":1.5,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"checkPhotosToReelsUpsellEligibility\":false,\"renderLocation\":\"group\",\"useDefaultActor\":false,\"inviteShortLinkKey\":null,\"isFeed\":false,\"isFundraiser\":false,\"isFunFactPost\":false,\"isGroup\":true,\"isEvent\":false,\"isTimeline\":false,\"isSocialLearning\":false,\"isPageNewsFeed\":false,\"isProfileReviews\":false,\"isWorkSharedDraft\":false,\"UFI2CommentsProvider_commentsKey\":\"CometGroupDiscussionRootSuccessQuery\",\"hashtag\":null,\"canUserManageOffers\":false,\"__relay_internal__pv__CometUFIIsRTAEnabledrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesRingrelayprovider\":false}}",
                    //["variables"] = $"{{\"input\":{{\"composer_entry_point\":\"inline_composer\",\"composer_source_surface\":\"group\",\"composer_type\":\"group\",\"logging\":{{\"composer_session_id\":\"526f7282-3302-4014-8ee1-a37c2ecdc5e1\"}},\"source\":\"WWW\",\"attachments\":[{{\"link\":{{\"share_scrape_data\":\"{{\\\"share_type\\\":22,\\\"share_params\\\":[{sharepr}]}}\"}}}}],\"message\":{{\"ranges\":[],\"text\":\"Hay\"}},\"with_tags_ids\":[],\"inline_activities\":[],\"explicit_place_id\":\"0\",\"text_format_preset_id\":\"0\",\"navigation_data\":{{\"attribution_id_v2\":\"CometGroupDiscussionRoot.react,comet.group,via_cold_start,1702439915702,670176,2361831622,,\"}},\"tracking\":[null],\"event_share_metadata\":{{\"surface\":\"newsfeed\"}},\"audience\":{{\"to_id\":\"{groupID}\"}},\"actor_id\":\"{_user}\",\"client_mutation_id\":\"1\"}},\"displayCommentsFeedbackContext\":null,\"displayCommentsContextEnableComment\":null,\"displayCommentsContextIsAdPreview\":null,\"displayCommentsContextIsAggregatedShare\":null,\"displayCommentsContextIsStorySet\":null,\"feedLocation\":\"GROUP\",\"feedbackSource\":0,\"focusCommentID\":null,\"gridMediaWidth\":null,\"groupID\":null,\"scale\":1,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"checkPhotosToReelsUpsellEligibility\":false,\"renderLocation\":\"group\",\"useDefaultActor\":false,\"inviteShortLinkKey\":null,\"isFeed\":false,\"isFundraiser\":false,\"isFunFactPost\":false,\"isGroup\":true,\"isEvent\":false,\"isTimeline\":false,\"isSocialLearning\":false,\"isPageNewsFeed\":false,\"isProfileReviews\":false,\"isWorkSharedDraft\":false,\"UFI2CommentsProvider_commentsKey\":\"CometGroupDiscussionRootSuccessQuery\",\"hashtag\":null,\"canUserManageOffers\":false,\"__relay_internal__pv__CometUFIIsRTAEnabledrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesRingrelayprovider\":false}}",
                    ["server_timestamps"] = "true",
                    ["doc_id"] = "7677593048935391",
                    ["fb_api_analytics_tags"] = "[\"qpl_active_flow_ids=431626709\"]",

                };
                FunctionHelper.AddHeaderxNet(rq, $@"sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                            DNT: 1
                                            sec-ch-ua-mobile: ?0
                                            viewport-width: 2510
                                            X-FB-Friendly-Name: ComposerStoryCreateMutation
                                            X-FB-LSD: {lsd}
                                            sec-ch-ua-platform-version: ""15.0.0""
                                            Content-Type: application/x-www-form-urlencoded
                                            X-ASBD-ID: 129477
                                            dpr: 1
                                            sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                            sec-ch-ua-model: """"
                                            sec-ch-prefers-color-scheme: dark
                                            sec-ch-ua-platform: ""Windows""
                                            Accept: */*
                                            Origin: https://www.facebook.com
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: cors
                                            Sec-Fetch-Dest: empty
                                            Referer: {refer}");
                try
                {
                    body = rq.Post($"https://www.facebook.com/api/graphql/", pr).ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                postID = RegexHelper.GetValueFromGroup("\"post_id\":\"(.*?)\",", body);
                if (postID == "")
                {
                    return ResultModel.Fail;
                }
                //Vào link post
                FunctionHelper.AddHeaderxNet(rq, @"Cache-Control: max-age=0
                                                dpr: 1
                                                viewport-width: 2510
                                                sec-ch-ua: ""Not_A Brand"";v=""8"", ""Chromium"";v=""120"", ""Microsoft Edge"";v=""120""
                                                sec-ch-ua-mobile: ?0
                                                sec-ch-ua-platform: ""Windows""
                                                sec-ch-ua-platform-version: ""15.0.0""
                                                sec-ch-ua-model: """"
                                                sec-ch-ua-full-version-list: ""Not_A Brand"";v=""8.0.0.0"", ""Chromium"";v=""120.0.6099.71"", ""Microsoft Edge"";v=""120.0.2210.61""
                                                sec-ch-prefers-color-scheme: dark
                                                DNT: 1
                                                Upgrade-Insecure-Requests: 1
                                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                Sec-Fetch-Site: none
                                                Sec-Fetch-Mode: navigate
                                                Sec-Fetch-User: ?1
                                                Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get($"https://www.facebook.com/{postID}").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                if (refer == $"https://www.facebook.com/{postID}")
                    return ResultModel.PostDeleted;
                group.C_PostID = postID;
                group.C_CreatedPost = DateTime.Now.ToString();
                group.C_UIDVia = account.C_UID;

                return ResultModel.Success;
            }
        }
        public bool LoginWWW(AccountModel account)
        {
            string body = "", refer = "", url = "", jazoest = "", lsd = "", login_source = "", submit = "", fb_dtsg = "", nh = "", _js_datr = "";
            RequestParams pr = null;

            using (var rq = new Leaf.xNet.HttpRequest())
            {
                rq.Cookies = new CookieStorage();
                rq.AllowAutoRedirect = true;
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                rq.UserAgent = account.C_UserAgent;
                //load trang chủ
                try
                {
                    FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        sec-ch-ua-mobile: ?0
                                        sec-ch-ua-platform: ""Windows""
                                        DNT: 1
                                        Upgrade-Insecure-Requests: 1
                                        Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                        Sec-Fetch-Site: none
                                        Sec-Fetch-Mode: navigate
                                        Sec-Fetch-User: ?1
                                        Sec-Fetch-Dest: document
                                        Accept-Language: en-US,en;q=0.9");

                    body = rq.Get("https://www.facebook.com/").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch { }

                _js_datr = RegexHelper.GetValueFromGroup("_js_datr\",\"(.*?)\"", body);
                if (_js_datr != "")
                {
                    rq.Cookies.Add(new System.Net.Cookie("_js_datr", _js_datr, "/", ".facebook.com"));
                    rq.Cookies.Add(new System.Net.Cookie("wd", "2550x1274", "/", ".facebook.com"));
                }

                //bấm accept all cookie
                if (body.Contains("accept_only_essential"))
                {
                    jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
                    lsd = RegexHelper.GetValueFromGroup("name=\"lsd\" value=\"(.*?)\"", body);

                    try
                    {
                        pr = new RequestParams
                        {
                            ["accept_only_essential"] = "false",
                            ["dpr"] = "1",
                            ["lsd"] = lsd,
                            ["jazoest"] = jazoest
                        };

                        FunctionHelper.AddHeaderxNet(rq, $@"accept: */*
                                            accept-language: en-US,en;q=0.9
                                            content-type: application/x-www-form-urlencoded
                                            referer: {refer}
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            sec-fetch-dest: empty
                                            sec-fetch-mode: cors
                                            sec-fetch-site: same-origin
                                            x-asbd-id: 129477
                                            x-fb-lsd: {lsd}");

                        rq.Post("https://www.facebook.com/cookie/consent/", pr);
                    }
                    catch { }
                }

                //bấm login
                url = RegexHelper.GetValueFromGroup("data-testid=\"royal_login_form\" action=\"(.*?)\"", body);
                if (url != "")
                {
                    jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
                    lsd = RegexHelper.GetValueFromGroup("name=\"lsd\" value=\"(.*?)\"", body);
                    login_source = RegexHelper.GetValueFromGroup("name=\"login_source\" value=\"(.*?)\"", body);

                    try
                    {
                        pr = new RequestParams
                        {
                            ["jazoest"] = jazoest,
                            ["lsd"] = lsd,
                            ["email"] = account.C_UID,
                            ["login_source"] = login_source,
                            ["next"] = "",
                            ["encpass"] = account.C_Password,
                        };

                        FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        Cache-Control: max-age=0
                                        sec-ch-ua-mobile: ?0
                                        sec-ch-ua-platform: ""Windows""
                                        Origin: https://www.facebook.com
                                        DNT: 1
                                        Upgrade-Insecure-Requests: 1
                                        Content-Type: application/x-www-form-urlencoded
                                        Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                        Sec-Fetch-Site: same-origin
                                        Sec-Fetch-Mode: navigate
                                        Sec-Fetch-User: ?1
                                        Sec-Fetch-Dest: document
                                        Referer: {refer}
                                        Accept-Language: en-US,en;q=0.9");

                        body = rq.Post("https://www.facebook.com" + url, pr).ToString();
                        refer = rq.Address.AbsoluteUri;
                    }
                    catch { }
                }

                rq.Cookies.Remove("https://www.facebook.com/", "_js_datr");

                if (refer.Contains("https://www.facebook.com/checkpoint/?next"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //check 2fa
                        if (body.Contains("approvals_code"))
                        {
                            submit = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("button value=\"(.*?)\" class=\"(.*?)\" id=\"checkpointSubmitButton\" name=\"submit\\[Continue\\]\"", body));
                            if (submit != "")
                            {
                                jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
                                fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
                                nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);

                                try
                                {
                                    pr = new RequestParams
                                    {
                                        ["jazoest"] = jazoest,
                                        ["fb_dtsg"] = fb_dtsg,
                                        ["nh"] = nh,
                                        ["approvals_code"] = FunctionHelper.ConvertTwoFA(account.C_2FA),
                                        ["submit[Continue]"] = submit
                                    };

                                    FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            Cache-Control: max-age=0
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            Origin: https://www.facebook.com
                                            DNT: 1
                                            Upgrade-Insecure-Requests: 1
                                            Content-Type: application/x-www-form-urlencoded
                                            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: navigate
                                            Sec-Fetch-User: ?1
                                            Sec-Fetch-Dest: document
                                            Referer: {refer}
                                            Accept-Language: en-US,en;q=0.9");

                                    body = rq.Post("https://www.facebook.com/checkpoint/?next", pr).ToString();
                                    refer = rq.Address.AbsoluteUri;
                                }
                                catch { }
                            }
                        }

                        //check continue
                        submit = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("button value=\"(.*?)\" class=\"(.*?)\" id=\"checkpointSubmitButton\" name=\"submit\\[Continue\\]\"", body));
                        if (submit != "")
                        {
                            jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
                            fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
                            nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);

                            try
                            {
                                pr = new RequestParams
                                {
                                    ["jazoest"] = jazoest,
                                    ["fb_dtsg"] = fb_dtsg,
                                    ["nh"] = nh,
                                    ["submit[Continue]"] = submit
                                };

                                FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            Cache-Control: max-age=0
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            Origin: https://www.facebook.com
                                            DNT: 1
                                            Upgrade-Insecure-Requests: 1
                                            Content-Type: application/x-www-form-urlencoded
                                            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: navigate
                                            Sec-Fetch-User: ?1
                                            Sec-Fetch-Dest: document
                                            Referer: {refer}
                                            Accept-Language: en-US,en;q=0.9");

                                body = rq.Post("https://www.facebook.com/checkpoint/?next", pr).ToString();
                                refer = rq.Address.AbsoluteUri;
                            }
                            catch { }
                        }

                        //check This was me
                        submit = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("button value=\"(.*?)\" class=\"(.*?)\" id=\"checkpointSubmitButton\" name=\"submit\\[This was me\\]\"", body));
                        if (submit != "")
                        {
                            jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
                            fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
                            nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);

                            try
                            {
                                pr = new RequestParams
                                {
                                    ["jazoest"] = jazoest,
                                    ["fb_dtsg"] = fb_dtsg,
                                    ["nh"] = nh,
                                    ["submit[This was me]"] = submit
                                };

                                FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            Cache-Control: max-age=0
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            Origin: https://www.facebook.com
                                            DNT: 1
                                            Upgrade-Insecure-Requests: 1
                                            Content-Type: application/x-www-form-urlencoded
                                            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: navigate
                                            Sec-Fetch-User: ?1
                                            Sec-Fetch-Dest: document
                                            Referer: {refer}
                                            Accept-Language: en-US,en;q=0.9");

                                body = rq.Post("https://www.facebook.com/checkpoint/?next", pr).ToString();
                                refer = rq.Address.AbsoluteUri;
                            }
                            catch { }
                        }

                        if (refer == "https://www.facebook.com/")
                        {
                            rq.Cookies.Remove("https://www.facebook.com/", "checkpoint");
                            break;
                        }
                    }
                }

                if (refer.Contains("956/"))
                {
                    FunctionHelper.EditValueColumn(account, "C_Status", "Login bị checkpoint 956", true);
                    return false;
                }

                if (refer.Contains("282/"))
                {
                    FunctionHelper.EditValueColumn(account, "C_Status", "Login bị checkpoint 282", true);
                    return false;
                }

                if (refer.Contains("checkpoint"))
                {
                    FunctionHelper.EditValueColumn(account, "C_Status", "Login bị checkpoint", true);
                    return false;
                }

                account.C_Cookie = rq.Cookies.GetCookieHeader("https://www.facebook.com/");
                FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
            }
            return true;
        }
    }
}
