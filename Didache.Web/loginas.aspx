<%@ Page Language="C#" %>
<script runat="server">
void Page_Load() {
		FormsAuthentication.SetAuthCookie(Request["username"].ToString(), true);
		Response.Redirect("/");
}
</script>