import React, {Component} from 'react';
import Tabs from 'arui-feather/tabs';
import TabItem from 'arui-feather/tab-item';
import Results from './results/results';
import Users from "./users/users";
import Questions from "./questions/questions";
import {connect} from "react-redux";
import {logUser, verifyToken} from "../actions/auth";
import Authenticated from "./auth/authenticated";
import {changeTab, loadTab} from "../actions/view";
import styled from "@emotion/styled";

const AppIsland = styled.div`
    height: 100%;
    background-color: #fcfcfc;
`;

const Header = styled.header`
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.16);
    background: rgba(11, 31, 53, .9);
    padding: 20px 30px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    box-sizing: border-box;
`;

const Main = styled.main`
    margin-left: 10px;
`;

const HeaderName = styled.h2`
    font-size: 24px;
    font-weight: 500;
    color: azure;
    margin: 0 0 0 10px;
`;

const TabItemStyled = styled(TabItem)`
    text-transform: uppercase;
    letter-spacing: 2px;
    font-size: 12px;
`;

class App extends Component {
    constructor(props) {
        super(props);

        this.pageIsActive = this.pageIsActive.bind(this);
    }

    componentDidMount() {
        this.props.verifyAuth();
        this.props.loadTab();
    };

    handleClick = event => {
        event.preventDefault();
        this.props.onTabChange(event.target.getAttribute('href'));
    };

    pageIsActive(currentPage) {
        return this.props.page === currentPage;
    }

    renderContent = () => {
        const {page, isAuth, onLogUserClick, authError} = this.props;

        if (page === '/results') {
            return <Results isActive={this.pageIsActive}/>
        }
        if (page === '/users') {
            return (
                <div>
                    <Authenticated isAuth={isAuth} logUser={onLogUserClick} authError={authError}>
                        <Users/>
                    </Authenticated>
                </div>
            )
        }
        if (page === '/questions') {
            return (
                <div>
                    <Authenticated isAuth={isAuth} logUser={onLogUserClick} authError={authError}>
                        <Questions/>
                    </Authenticated>
                </div>
            )
        }
    };

    render() {
        const {page} = this.props;
        return (
            <AppIsland>
                <Header>
                    <img src={"img/icon_bank.svg"} alt="logo"/>
                    <HeaderName> Альфа-Банк Викторина</HeaderName>
                </Header>
                <Main>
                    <Tabs>
                        <TabItemStyled url='/results'
                                 onClick={this.handleClick}
                                 checked={page === '/results'}>
                            Результаты
                        </TabItemStyled>
                        <TabItemStyled url='/users' onClick={this.handleClick} checked={page === '/users'}>
                            Пользователи
                        </TabItemStyled>
                        <TabItemStyled url='/questions' onClick={this.handleClick} checked={page === '/questions'}>
                            Вопросы
                        </TabItemStyled>
                    </Tabs>
                    {this.renderContent()}
                </Main>
            </AppIsland>
        );
    }
}

const mapsStateToProps = state => ({
    isAuth: state.auth.isAuth,
    authError: state.auth.error,
    page: state.view.activeTab
});

const mapDispatchToProps = dispatch => ({
    verifyAuth: () => dispatch(verifyToken()),
    onLogUserClick: (user, pass) => dispatch(logUser(user, pass)),
    onTabChange: page => dispatch(changeTab(page)),
    loadTab: () => dispatch(loadTab())
});

export default connect(mapsStateToProps, mapDispatchToProps)(App);
