import React, {Component} from 'react';
import Heading from 'arui-feather/heading';
import {connect} from "react-redux";
import {fetchUsers} from "../actions/users";
import {loadToken, setToken} from "../actions/token";
import FormField from "arui-feather/form-field";
import Input from "arui-feather/input";
import Button from "arui-feather/button";
import Label from "arui-feather/label";
import Table from "./table";

class Users extends Component {
    constructor(props) {
        super(props);
        this.state = {
            tokenValueField: null
        };

        this.renderTokenForm = this.renderTokenForm.bind(this);
        this.handleClick = this.handleClick.bind(this);
        this.handleChange = this.handleChange.bind(this);
        Users.renderNotification = Users.renderNotification.bind(this);
    }

    componentDidMount() {
        const {loadToken, fetchUsers, token} = this.props;
        loadToken();
        if (token) {
            fetchUsers(token);
        }
    }

    handleClick() {
        this.props.setToken(this.state.tokenValueField)
    }

    handleChange(e) {
        this.setState({
            tokenValueField: e
        });
    }

    renderTokenForm() {
        if (this.props.token === null || this.props.error === "Unauthorized") return (
            <div>
                {Users.renderNotification(this.props.error)}
                <FormField>
                    <Input placeholder='Введите token' onChange={e => this.handleChange(e)}/>
                </FormField>
                <Button view='extra' onClick={this.handleClick}>Отправить</Button>

            </div>);
    }

    static renderNotification(error) {
        const elipsisBoxStyles = {
            width: '200px',
            overflow: 'hidden',
            textOverflow: 'ellipsis'
        };
        const redColor = {color: "red"};

        if (error === "Unauthorized")
            return (
                <div style={elipsisBoxStyles}>
                    <Label style={redColor} size="m" isNoWrap={true}>
                        {error}
                    </Label>
                </div>
            );
    }

    static renderData(users) {
        if (users) return <Table users={users}/>;
    }

    render() {
        const {users} = this.props;
        return (
            <div>
                <Heading size='m'>
                    Список пользователей
                </Heading>
                {this.renderTokenForm()}
                {Users.renderData(users)}
            </div>
        );
    }
}

const mapStateToProps = state => ({
    users: state.users.data,
    error: state.users.error,
    token: state.token.token
});

const mapDispatchToProps = dispatch => ({
    fetchUsers: token => dispatch(fetchUsers(token)),
    setToken: token => dispatch(setToken(token)),
    loadToken: () => dispatch(loadToken())
});

export default connect(mapStateToProps, mapDispatchToProps)(Users);