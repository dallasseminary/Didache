<%@ Page Language="C#" %>
<script runat="server">
void Page_Load() {
	FormsAuthentication.SignOut();
}
</script>