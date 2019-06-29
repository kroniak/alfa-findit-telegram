export default ({isAuth, children}) => {
    if (isAuth && children) return children;
    else return null;
};