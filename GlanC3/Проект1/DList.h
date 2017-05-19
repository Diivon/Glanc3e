#pragma once


//---------------------------------------
template<class T, class Allocator, class Y>
DList<T, Allocator>::iterator DList<T, Allocator>::emplaceBack(Y && y) {
	if (!_first) {//if list is empty
		auto block = Allocator::allocate(bytes(sizeof(decltype(*_first))));
		if (!block.begin) throw std::bad_alloc();	//if no memory - throw
		_first = static_cast<decltype(_first)>(block.begin);
		IF_FAIL(new(&_first->_data) T(std::forward<Y>(y))) {
			Allocator::deallocate(block); _first = nullptr; throw;
		}//like nothing is happened
		 //if no incidents - continue construct
		_first->_next = _first->_prev = nullptr;
		_first->_mem = block;
		_last = _first;
		++_len;
	}
	else {//we already got some elements
		auto block = Allocator::allocate(bytes(sizeof(decltype(*_first))));
		if (!block.begin) throw std::bad_alloc();//if no memory - throw
		_last->_next = static_cast<decltype(_first)>(block.begin);
		IF_FAIL(new(&_last->_next->_data) T(std::forward<Y>(y))) {
			Allocator::deallocate(block); _last->_next = nullptr; throw;
		}//like nothing is happened
		_last->_next->_prev = _last;
		_last = _last->_next;
		_last->_next = nullptr;
		_last->_mem = block;
		++_len;
	}
	return iterator(_last);
}
template<class T, class Allocator>
DList<T, Allocator>::lref_t DList<T, Allocator>::popBack() {
	_last->_data->~T();
	_last = _last->_prev;
	Allocator::deallocate(_last->_next->_mem);
	--_len;
}
template<class T, class Allocator, class Callback>
DList<T, Allocator> DList<T, Allocator>::map(Callback && f) const noexcept(noexcept(f(T&))) {
	this_t result;
	for (auto node = _first; node != nullptr; node = node->_next)
		result.emplaceBack(f(node->_data));
	return result;
}
template<class T, class Allocator, class Callback>
DList<T, Allocator>::lref_t DList<T, Allocator>::foreach(Callback && f) {
	for (auto node = _first; node != nullptr; node = node->_next)
		f(node->_data);
}
template<class T, class Allocator>
DList<T, Allocator>::lref_t DList<T, Allocator>::join(DList<T, Allocator>::lref_t other) {
	_last->_next = other._first;
	other._first->_prev = _last;
}
template<class T, class Allocator, class Y>
DList<T, Allocator>::lref_t DList<T, Allocator>::operator << (Y && y) {
	emplaceBack(std::forward<Y>(y));
}
template<class T, class Allocator>
DList<T, Allocator>::iterator DList<T, Allocator>::begin(DList<T, Allocator>::lref_t other) {
	return iterator(_first);
}
template<class T, class Allocator>
DList<T, Allocator>::iterator DList<T, Allocator>::preBegin(DList<T, Allocator>::lref_t other) {
	return iterator(_preBeginPtr);
}
template<class T, class Allocator>
DList<T, Allocator>::iterator DList<T, Allocator>::end(DList<T, Allocator>::lref_t other) {
	return iterator(_endPtr);
}
template<class T, class Allocator>
DList<T, Allocator>::iterator DList<T, Allocator>::last(DList<T, Allocator>::lref_t other) {
	return iterator(_last);
}