using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using ChineseSchool.Models;


namespace ChineseSchool.Extensions
{
    public static class HtmlExtensions
    {

       

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, PageInfo pageInfo)
        {

            int start = ((pageInfo.CurrentPage - 1) / 10) * 10 + 1;
            int end;
            if (pageInfo.PageCount < (start + 9))
            {
                end = pageInfo.PageCount;
            }
            else
            {
                end = start + 9;
            }
            TagBuilder divBuilder = new TagBuilder("div");
            divBuilder.MergeAttribute("class", "pagination_div");
            TagBuilder spanBuilder = new TagBuilder("span");
            spanBuilder.GenerateId("pagenav");
            TagBuilder firstPageHrefBuilder = new TagBuilder("a");
            if (pageInfo.CurrentPage != 1)
            {
                firstPageHrefBuilder.MergeAttribute("href", pageInfo.Url + "1");
            }
            firstPageHrefBuilder.InnerHtml = "|<<";
            spanBuilder.InnerHtml += firstPageHrefBuilder;
            TagBuilder ahrefBuilder = new TagBuilder("a");

            if (pageInfo.CurrentPage > 1)
            {
                ahrefBuilder.MergeAttribute("href", pageInfo.Url + (pageInfo.CurrentPage - 1).ToString());
            }
            ahrefBuilder.InnerHtml = "prev";
            spanBuilder.InnerHtml += ahrefBuilder;
            for (int i = start; i <= end; i++)
            {
                TagBuilder aBuilder = new TagBuilder("a");
                if (i != pageInfo.CurrentPage)
                {
                    aBuilder.MergeAttribute("href", pageInfo.Url + i);
                }
                else
                {
                    aBuilder.MergeAttribute("class", "pagecurrent");
                }
                aBuilder.InnerHtml = i.ToString();
                spanBuilder.InnerHtml += aBuilder;
            }
            TagBuilder ahrefBuilder1 = new TagBuilder("a");

            if (pageInfo.CurrentPage < pageInfo.PageCount)
            {
                ahrefBuilder1.MergeAttribute("href", pageInfo.Url + (pageInfo.CurrentPage + 1).ToString());
            }
            ahrefBuilder1.InnerHtml = "next";
            spanBuilder.InnerHtml += ahrefBuilder1;
            TagBuilder lastPageHrefBuilder = new TagBuilder("a");
            if (pageInfo.CurrentPage != pageInfo.PageCount)
            {
                lastPageHrefBuilder.MergeAttribute("href", pageInfo.Url + pageInfo.PageCount.ToString());
            }
            lastPageHrefBuilder.InnerHtml = ">>|";
            spanBuilder.InnerHtml += lastPageHrefBuilder;
            TagBuilder spanBuilder1 = new TagBuilder("span");
            spanBuilder1.GenerateId("pagejumper_span");
            spanBuilder1.InnerHtml = "Page: ";
            TagBuilder selectBuilder = new TagBuilder("select");
            selectBuilder.GenerateId("pagejumper_select");
            for (int i = 1; i <= pageInfo.PageCount; i++)
            {
                TagBuilder optionBuilder = new TagBuilder("option");
                optionBuilder.MergeAttribute("value", i.ToString());
                if (i == pageInfo.CurrentPage)
                {
                    optionBuilder.MergeAttribute("selected", "selected");
                }
                optionBuilder.InnerHtml = i.ToString();
                selectBuilder.InnerHtml += optionBuilder;
            }
            spanBuilder1.InnerHtml += selectBuilder;
            spanBuilder1.InnerHtml += (" of " + pageInfo.PageCount.ToString());
            divBuilder.InnerHtml += spanBuilder;
            divBuilder.InnerHtml += spanBuilder1;
            TagBuilder jsBuilder = new TagBuilder("script");
            jsBuilder.InnerHtml = "$(document).ready(function(){$('#pagejumper_select').change(function(){var pageToJump=$('#pagejumper_select').val();url='" + pageInfo.Url + "'+pageToJump+''; window.location=url})})";
            divBuilder.InnerHtml += jsBuilder;
            return MvcHtmlString.Create(divBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString GenderSelectList(this HtmlHelper hemlHelper, String selectedGender )
        {
            TagBuilder builder = new TagBuilder("select");
            builder.MergeAttribute("name", "Gender");
            TagBuilder optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "M");
            if (selectedGender=="M")
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.InnerHtml = "Male";
            builder.InnerHtml += optionBuilder;
            optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "F");
            if (selectedGender == "F")
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.InnerHtml = "Female";
            builder.InnerHtml += optionBuilder;
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }
        public static MvcHtmlString EnrichmentClassSelectList(this HtmlHelper htmlHelper, List<ChineseSchool.Entities.EnrichmentClass> enrichmentClasses, int selectedItemId = 0,string name="EnrichmentClass", string defaultMsg = "DO NOT REGISTER")
        {
            TagBuilder selectBuilder = new TagBuilder("select");
            selectBuilder.MergeAttribute("name", name);
            TagBuilder optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "0");
            if (selectedItemId == 0)
            {

                optionBuilder.MergeAttribute("selected", "selected");
                //optionBuilder.MergeAttribute("disabled", "disabled");

            }
            optionBuilder.InnerHtml = defaultMsg;
            selectBuilder.InnerHtml += optionBuilder;
                
             
                
            
                
      

            
            
            foreach (var item in enrichmentClasses)
            {
                optionBuilder = new TagBuilder("option");
                if (item.ClassID == selectedItemId)
                {
                    optionBuilder.MergeAttribute("selected", "selected");
                }
                optionBuilder.MergeAttribute("value", item.ClassID.ToString());
                optionBuilder.InnerHtml = item.ClassName;
                selectBuilder.InnerHtml += optionBuilder;
            }
            return MvcHtmlString.Create(selectBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ClassSelectList(this HtmlHelper htmlHelper, List<ChineseSchool.Entities.Class> classes, int selectedItemId = 0, string name = "Class", string defaultMsg = "Not Assigned")
        {
            TagBuilder selectBuilder = new TagBuilder("select");
            selectBuilder.MergeAttribute("name", name);
            TagBuilder optionBuilder = new TagBuilder("option");

           
                optionBuilder.MergeAttribute("value", "");
                if (selectedItemId == 0 || selectedItemId == null)
                {
                    optionBuilder.MergeAttribute("selected", "selected");
                }
                //optionBuilder.MergeAttribute("disabled", "disabled");
                optionBuilder.InnerHtml = defaultMsg;
                selectBuilder.InnerHtml += optionBuilder;
            
            foreach (var item in classes)
            {
                optionBuilder = new TagBuilder("option");
                if (item.ClassId == selectedItemId)
                {
                    optionBuilder.MergeAttribute("selected", "selected");
                }
                optionBuilder.MergeAttribute("value", item.ClassId.ToString());
                optionBuilder.InnerHtml = item.Classname;
                selectBuilder.InnerHtml += optionBuilder;
            }
            return MvcHtmlString.Create(selectBuilder.ToString(TagRenderMode.Normal));
        }
        public static MvcHtmlString PositionSelectList(this HtmlHelper htmlHtlper, List<ChineseSchool.Entities.Position> positons, int selectedItemId = 0, string name = "PositionId")
        {
            TagBuilder selectBuilder = new TagBuilder("select");
            selectBuilder.MergeAttribute("name", name);
            TagBuilder optionBuilder = new TagBuilder("option");


            optionBuilder.MergeAttribute("value", "");
            if (selectedItemId == 0 || selectedItemId == null)
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.MergeAttribute("disabled", "disabled");
            optionBuilder.InnerHtml = "Please Select";
            selectBuilder.InnerHtml += optionBuilder;

            foreach (var item in positons)
            {
                optionBuilder = new TagBuilder("option");
                if (item.PositionID == selectedItemId)
                {
                    optionBuilder.MergeAttribute("selected", "selected");
                }
                optionBuilder.MergeAttribute("value", item.PositionID.ToString());
                optionBuilder.InnerHtml = item.PositionName;
                selectBuilder.InnerHtml += optionBuilder;
            }
            return MvcHtmlString.Create(selectBuilder.ToString(TagRenderMode.Normal));
        }
        public static MvcHtmlString TransactionTypeSelectList(this HtmlHelper htmlHelper, String selectedType)
        {
            TagBuilder builder = new TagBuilder("select");
            builder.MergeAttribute("name", "TransactionType");
            TagBuilder optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "Charge");
            if (selectedType == "Charge")
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.InnerHtml = "Charge";
            builder.InnerHtml += optionBuilder;
            optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "Payment");
            if (selectedType == "Payment")
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.InnerHtml = "Payment";
            builder.InnerHtml += optionBuilder;
            optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "Refund");
            if (selectedType == "Refund")
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.InnerHtml = "Refund";
            builder.InnerHtml += optionBuilder;
            optionBuilder = new TagBuilder("option");
            optionBuilder.MergeAttribute("value", "Adjustment");
            if (selectedType == "Adjustment")
            {
                optionBuilder.MergeAttribute("selected", "selected");
            }
            optionBuilder.InnerHtml = "Adjustment";
            builder.InnerHtml += optionBuilder;
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }
    }

    



    public static class ExceptionExtensions
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }
    }
}