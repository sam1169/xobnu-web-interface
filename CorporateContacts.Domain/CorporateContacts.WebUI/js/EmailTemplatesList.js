function initEmailTemplatesList() {
    // chosen plugin inside a modal will have a zero width because the select element is originally hidden
    // and its width cannot be determined.
    // so we set the width after modal is show
    $('#modal-form').on('shown.bs.modal', function () {
        $(this).find('.chosen-container').each(function () {
            $(this).find('a:first-child').css('width', '210px');
            $(this).find('.chosen-drop').css('width', '210px');
            $(this).find('.chosen-search input').css('width', '200px');
        });
    });
    createEmailsTemplateGrid();
}
function showAddTemplatePopup() {
    $("#modal-content").html("");
    $("#modal-content").load("/Email/EmailTemplates/EmailTemplateAddEdit/EmailTemplateAddEdit.html", function () {
        $("#form-field-input-id").val(0);
        $("#modal-header-title").html("Add Email Template");
    });
}
function editEmailTemplate(templateId) {
    $("#modal-content").html("");
    var rowData = $('#grid-table').jqGrid('getRowData', templateId);
    $("#modal-content").load("/Email/EmailTemplates/EmailTemplateAddEdit/EmailTemplateAddEdit.html", function () {
        setEmailTemplatePopupTitle("Edit Email Template");
        setEmailTemplateId(rowData.EmailTemplateId);
        setEmailTemplateEmailTypeId(rowData.EmailTypeId);
        setEmailTemplateSubject(rowData.TemplateSubject);
        setEmailTemplateFromName(rowData.FromName);
        setEmailTemplateFromEmail(rowData.FromEmail);
        setEmailTemplateEmailSignature(rowData.EmailSignature);
        setEmailTemplateBody(rowData.TemplateBody);
        setEmailTemplateSubscriberId(rowData.SubscriberId);
        loadEmailTypes(rowData.EmailTypeId);
        loadSubscribersDropDown(rowData.SubscriberId);
        if (rowData.Tags != "") {
            setEmailTemplateTags(rowData.Tags);
        }
        var weeklyEmailTypeId = getEmailTemplateWeekyEmailTypeId();
        var selectedEmailType = getEmailTemplateSelectedEmailType();
        if (weeklyEmailTypeId == selectedEmailType) {
            //Set values for 'weekly email' dynamic section
            var sendAtDateTime = rowData.SendAtTime;
            if (sendAtDateTime != "undefined" || sendAtDateTime != "") {
                var sendAtTime = sendAtDateTime.split('T')[1];
                setSendAtTime(sendAtTime);
            }
            var userRole = rowData.UserRole;
            if (userRole != "undefined" || userRole != "") {
                loadUserRolesDropDown(userRole);
            }
            var includes = rowData.WeeklyEmailIncludes;
            if (includes != "undefined" || includes != "") {
                loadWeeklyEmailIncludes(includes);
            }
            var day = rowData.SendOnDay;
            if (day != "undefined" || day != "") {
                setSendOnDay(day);
            }
        }
    });
}
function createEmailsTemplateGrid() {   
    $("#noRecordsMessageContainer").hide();
    var vUrl = 'List';
    var vColumnHeaderNames = [
        "Given Name", "Last Name", "Job Title", "Company Name", "Mobile Telephone Number", "Business Telephone Number", "Email1 Address"
    ];
    jQuery(function ($) {
        var gridSelector = "#grid-table";
        var pagerSelector = "#grid-pager";
        // resize to fit page size
        $(window).on('resize.jqGrid', function () {
            $(gridSelector).jqGrid('setGridWidth', $(".page-content").width());
        });
        // resize on sidebar collapse/expand
        var parentColumn = $(gridSelector).closest('[class*="col-"]');
        $(document).on('settings.ace.jqGrid', function (ev, eventName) {
            if (eventName === 'sidebar_collapsed' || eventName === 'main_container_fixed') {
                // setTimeout is for webkit only to give time for DOM changes and then redraw!!!
                setTimeout(function () {
                    $(gridSelector).jqGrid('setGridWidth', parentColumn.width());
                }, 0);
            }
        });
        jQuery(gridSelector).jqGrid({
            url: vUrl,
            datatype: 'json',
            type: 'GET',
            height: "auto",
            colNames: vColumnHeaderNames,
            colModel: [               
                { name: 'GivenName', index: 'GivenName', sortable: true },
                { name: 'LastName', index: 'LastName', hidden: false, resize: true },
                { name: 'JobTitle', index: 'JobTitle', hidden: false, sortable: true },
                { name: 'CompanyName', index: 'CompanyName', resize: true },
                { name: 'MobilePhone', index: 'MobilePhone', resize: true },
                { name: 'BusinessPhone', index: 'BusinessPhone', hidden: false, resize: true },
                { name: 'EmailAddress1', index: 'EmailAddress1', hidden: false }
            ],
            viewrecords: true,
            viewpages: true,
            gridComplete: function () {
                var ids = $('#grid-table').jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var emailTemplateId = ids[i];
                    var rowHtml = "<a href='#modal-form' data-toggle='modal' onclick='editEmailTemplate(" + emailTemplateId + ")' class='blue'>Edit</a>";
                    $('#grid-table').jqGrid('setRowData', ids[i], { Action: rowHtml });
                }
                var recs = parseInt($("#grid-table").getGridParam("records"), 10);
                if (isNaN(recs) || recs == 0) {
                    $("#emailTemplatesGridContainer").hide();
                    $("#noRecordsMessageContainer").show();
                } else {
                    $('#emailTemplatesGridContainer').show();
                    $("#noRecordsMessageContainer").hide();
                }
                $("#load_grid-table").hide();
            },
            jsonReader: {
                root: 'ItemsTemplatesList',
                id: 'EmailTemplateId',
                records: 'Records',
                rows: 'PageSize',
                page: 'PageIndex',
                total: 'TotalPages',
                userdata: 'IsSessionExpired',
                repeatitems: false
            },
            rowNum: 25,
            rowList: [25, 50, 100],
            pager: pagerSelector,
            altRows: true,
            multiselect: true,
            multiboxonly: true,
            loadonce: false,
            beforeRequest: function () {
                var p = this.p, pd = p.postData;
                p.url = 'List'
                    + '?pageNumber=' + pd.page
                    + '&recordsPerPage=' + pd.rows
                    + '&sortColumn=' + encodeURIComponent(pd.sidx)
                    + '&sortOrder=' + pd.sord + '&sortBy=';
                p.postData = {};
                $("#load_grid-table").text("Retrieving Email Templates...");
            },
            //loadBeforeSend: function (jqXhr) {
            //    var token = $.cookie('TokenValue');
            //    if (token == 'undefined' || token == null) {
            //        window.location.href = "login.html";
            //    }
            //    jqXhr.setRequestHeader("Authorization", 'Basic ' + token);
            //},
            loadComplete: function () {
                //var isExpired = $("#grid-table").getGridParam('userData');
                //if (isExpired == "true") {
                //    $.cookie('RedirectLayoutName', 'emailtemplates');
                //    window.location.href = "login.html";
                //}
                setTimeout(function () {
                    updatePagerIcons();
                    enableTooltips();
                }, 0);
                $("#emailTemplatesGridContainer div").each(function () {
                    if (!$(this).hasClass("ui-pg-div") && !$(this).hasClass("loading")) {
                        if ($(this).hasClass("ui-jqgrid-hbox")) {
                            $(this).attr('style', 'width:100%;padding-right:0;');
                        } else {
                            $(this).attr('style', 'width:100%;');
                        }
                    }
                });
                $("#emailTemplatesGridContainer table").each(function () {
                    if (!$(this).hasClass("navtable")) {
                        if ($(this).hasClass("ui-pg-table")) {
                            $("#grid-pager_left").attr('style', 'width:35%');
                            $(this).attr('style', 'width:100%;margin-top:5px;');
                        } else {
                            $(this).attr('style', 'width:100%');
                        }
                    }
                });
                $("#emailTemplatesGridContainer").css({ "visibility": "visible" });
            },
            //caption: "Email Templates",
            autowidth: true
        });
        $(window).triggerHandler('resize.jqGrid');// trigger window resize to make the grid get the correct size
        // navButtons
        jQuery(gridSelector).jqGrid('navGrid', pagerSelector,
            {
                // navbar options
                edit: false,
                editicon: 'ace-icon fa fa-pencil blue',
                add: false,
                addicon: 'ace-icon fa fa-plus-circle purple',
                del: false,
                delicon: 'ace-icon fa fa-trash-o red',
                search: true,
                searchicon: 'ace-icon fa fa-search orange',
                refresh: true,
                refreshicon: 'ace-icon fa fa-refresh green',
                view: true,
                viewicon: 'ace-icon fa fa-search-plus grey',
            },
            {
                // edit record form
                // closeAfterEdit: true,
                // width: 700,
                recreateForm: true,
                beforeShowForm: function (e) {
                    var form = $(e[0]);
                    form.closest('.ui-jqdialog').find('.ui-jqdialog-titlebar').wrapInner('<div class="widget-header" />');
                    styleEditForm(form);
                }
            },
            {
                // new record form
                // width: 700,
                closeAfterAdd: true,
                recreateForm: true,
                viewPagerButtons: false,
                beforeShowForm: function (e) {
                    var form = $(e[0]);
                    form.closest('.ui-jqdialog').find('.ui-jqdialog-titlebar')
                        .wrapInner('<div class="widget-header" />');
                    styleEditForm(form);
                }
            },
            {
                // delete record form
                recreateForm: true,
                beforeShowForm: function (e) {
                    var form = $(e[0]);
                    if (form.data('styled')) return false;

                    form.closest('.ui-jqdialog').find('.ui-jqdialog-titlebar').wrapInner('<div class="widget-header" />');
                    styleDeleteForm(form);

                    form.data('styled', true);
                    return true;
                },
                onClick: function (e) {
                    alert(e);
                }
            },
            {
                // search form
                recreateForm: true,
                afterShowSearch: function (e) {
                    var form = $(e[0]);
                    form.closest('.ui-jqdialog').find('.ui-jqdialog-title').wrap('<div class="widget-header" />');
                    styleSearchForm(form);
                },
                afterRedraw: function () {
                    styleSearchFilters($(this));
                },
                multipleSearch: true,
                /**
                multipleGroup:true,
                showQuery: true
                */
            },
            {
                // view record form
                recreateForm: true,
                beforeShowForm: function (e) {
                    var form = $(e[0]);
                    form.closest('.ui-jqdialog').find('.ui-jqdialog-title').wrap('<div class="widget-header" />');
                }
            }
        );
        function styleEditForm(form) {
            // enable datepicker on "sdate" field and switches for "stock" field
            form.find('input[name=sdate]').datepicker({ format: 'yyyy-mm-dd', autoclose: true })
                .end().find('input[name=stock]')
                .addClass('ace ace-switch ace-switch-5').wrap('<label class="inline" />').after('<span class="lbl"></span>');
            // update buttons classes
            var buttons = form.next().find('.EditButton .fm-button');
            buttons.addClass('btn btn-sm').find('[class*="-icon"]').remove(); //ui-icon, s-icon
            buttons.eq(0).addClass('btn-primary').prepend('<i class="icon-ok"></i>');
            buttons.eq(1).prepend('<i class="icon-remove"></i>');
            buttons = form.next().find('.navButton a');
            buttons.find('.ui-icon').remove();
            buttons.eq(0).append('<i class="icon-chevron-left"></i>');
            buttons.eq(1).append('<i class="icon-chevron-right"></i>');
        }
        function styleDeleteForm(form) {
            var buttons = form.next().find('.EditButton .fm-button');
            buttons.addClass('btn btn-sm').find('[class*="-icon"]').remove(); //ui-icon, s-icon
            buttons.eq(0).addClass('btn-danger').prepend('<i class="icon-trash"></i>');
            buttons.eq(1).prepend('<i class="icon-remove"></i>');
        }
        function styleSearchFilters(form) {
            form.find('.delete-rule').val('X');
            form.find('.add-rule').addClass('btn btn-xs btn-primary');
            form.find('.add-group').addClass('btn btn-xs btn-success');
            form.find('.delete-group').addClass('btn btn-xs btn-danger');
        }
        function styleSearchForm(form) {
            var dialog = form.closest('.ui-jqdialog');
            var buttons = dialog.find('.EditTable');
            buttons.find('.EditButton a[id*="_reset"]').addClass('btn btn-sm btn-info').find('.ui-icon').attr('class', 'icon-retweet');
            buttons.find('.EditButton a[id*="_query"]').addClass('btn btn-sm btn-inverse').find('.ui-icon').attr('class', 'icon-comment-alt');
            buttons.find('.EditButton a[id*="_search"]').addClass('btn btn-sm btn-purple').find('.ui-icon').attr('class', 'icon-search');
        }
        // it causes some flicker when reloading or navigating grid
        // it may be possible to have some custom formatter to do this as the grid is being created to prevent this
        // or go back to default browser checkbox styles for the grid
        // ReSharper disable once UnusedLocals
        function styleCheckbox() {
            /**
                $(table).find('input:checkbox').addClass('ace')
                .wrap('<label />')
                .after('<span class="lbl align-top" />')
        
        
                $('.ui-jqgrid-labels th[id*="_cb"]:first-child')
                .find('input.cbox[type=checkbox]').addClass('ace')
                .wrap('<label />').after('<span class="lbl align-top" />');
            */
        }
        // unlike navButtons icons, action icons in rows seem to be hard-coded
        // you can change them like this in here if you want
        // ReSharper disable once UnusedLocals
        function updateActionIcons() {
            /**
            var replacement = 
            {
                'ui-icon-pencil' : 'icon-pencil blue',
                'ui-icon-trash' : 'icon-trash red',
                'ui-icon-disk' : 'icon-ok green',
                'ui-icon-cancel' : 'icon-remove red'
            };
            $(table).find('.ui-pg-div span.ui-icon').each(function(){
                var icon = $(this);
                var $class = $.trim(icon.attr('class').replace('ui-icon', ''));
                if($class in replacement) icon.attr('class', 'ui-icon '+replacement[$class]);
            })
            */
        }
        // replace icons with FontAwesome icons like above
        function updatePagerIcons() {
            var replacement =
            {
                'ui-icon-seek-first': 'ace-icon fa fa-angle-double-left bigger-140',
                'ui-icon-seek-prev': 'ace-icon fa fa-angle-left bigger-140',
                'ui-icon-seek-next': 'ace-icon fa fa-angle-right bigger-140',
                'ui-icon-seek-end': 'ace-icon fa fa-angle-double-right bigger-140'
            };
            $('.ui-pg-table:not(.navtable) > tbody > tr > .ui-pg-button > .ui-icon').each(function () {
                var icon = $(this);
                var $class = $.trim(icon.attr('class').replace('ui-icon', ''));

                if ($class in replacement) icon.attr('class', 'ui-icon ' + replacement[$class]);
            });
        }
        function enableTooltips(table) {
            $('.navtable .ui-pg-button').tooltip({ container: 'body' });
            $(table).find('.ui-pg-div').tooltip({ container: 'body' });
        }
        //var selr = jQuery(grid_selector).jqGrid('getGridParam','selrow');
    });
}
function closeEmailTemplateAddEditPopup() {
    $("#modal-form").hide();
    $(".modal-backdrop").css('opacity', 0);
    $(".modal-backdrop").css('z-index', -1);
}