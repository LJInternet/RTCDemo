#pragma once

namespace LJ
{

/**
 * This class is used in SharedPtr to delete a object pointer.
 */
template <class T>
class PtrDisposer
{
public:
    /**
     * Delete pointer ptr.
     * @param ptr pointer to be deleted.
     */
    virtual void dispose(T *ptr) = 0;
};

}
