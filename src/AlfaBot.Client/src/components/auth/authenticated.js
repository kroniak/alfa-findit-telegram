import LoginForm from "./login_form";
import React from "react";

export default ({isAuth, children, logUser, authError}) => {
    if (isAuth && children) return children;
    else return <LoginForm logUser={logUser} error={authError}/>;
};