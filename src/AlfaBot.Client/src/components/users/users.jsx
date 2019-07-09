import React, {Component} from 'react';
import {connect} from "react-redux";
import {fetchUsers} from "../../actions/users";
import Table from "./table";
import Button from "arui-feather/button/";
import Heading from 'arui-feather/heading/';
import {SpinCustom as Spin} from "../misc/misc";
import Notification from "../misc/notification";

const renderData = users => {
    if (users) return <Table users={users}/>;
    else return null;
};

class UsersContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLoading: false
        }
    }

    componentWillReceiveProps(nextProps, nextContext) {
        if (nextProps.isLoading) {
            this.setState({
                isLoading: true
            });
            setTimeout(() => {
                this.setState({
                    isLoading: false
                });
            }, 500);
        }
    }

    componentDidMount() {
        this.props.fetchUsers();
    }

    render() {
        const {users, fetchUsers, error} = this.props;
        const {isLoading} = this.state;
        return (
            <div>
                <Notification
                    title="Ошибка закгрузки"
                    message="Что то пошло не так! Возможно нет связи с сервером"
                    error={error}
                />
                <Heading size='m'>
                    Список пользователей
                </Heading>
                <Button disabled={isLoading}
                        view='extra'
                        size='m'
                        onClick={fetchUsers}>
                    Обновить
                </Button>
                <Spin
                    size='m'
                    visible={isLoading}
                />
                {renderData(users)}
            </div>
        );
    }
}

const mapStateToProps = state => ({
    users: state.users.data,
    error: state.users.error,
    isLoading: state.users.isLoading
});

const mapDispatchToProps = dispatch => ({
    fetchUsers: () => dispatch(fetchUsers()),
});

export default connect(mapStateToProps, mapDispatchToProps)(UsersContainer);