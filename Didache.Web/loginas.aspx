<%@ Page Language="C#" %>
<script runat="server">
void Page_Load() {
	FormsAuthentication.RedirectFromLoginPage(Request["username"].ToString(), true);
}
</script>